using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;
using xampl.Models.Documents;
using xampl.Models.DTO;
using xampl.Services.EmailSenderService;
using xampl.Services.RepositoryService;
using xampl.Utils;
using xampl.ViewModels;

namespace xampl.Controllers
{
    public class AccountController(
        IRepository<DocumentsContext> documentsRepository,
        EmailSender emailSender
    ) : Controller
    {
        private readonly IRepository<DocumentsContext> _documentsRepository = documentsRepository;
        private readonly EmailSender _emailSender = emailSender;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            var passwordHash = AccountUtils.ConvertToMD5(loginVM.Password);
            var user = await _documentsRepository.GetAllAsQueryable<User>().FirstOrDefaultAsync(x => x.Email == loginVM.Email);
            if (user is null)
            {
                ToastUtils.SetData(TempData, "No user registered with specified email", true);
                return RedirectToAction("Index", "About");
            }
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                loginVM.Password = string.Empty;
                return View(nameof(CreatePassword), loginVM);
            }
            if (user.PasswordHash == passwordHash)
            {
                //TODO: move this logic to claims transformer;
                //TODO: add JWT token;
                var claims = new List<Claim>
                {
                    new(ClaimTypes.GivenName, user.FirstName),
                    new(ClaimTypes.Surname, user.LastName),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.NameIdentifier, $"{user.Id} - {user.FirstName} {user.LastName}"),
                    new(ClaimTypes.AuthenticationMethod, "Manual")
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction("Index", "About");
            }

            ToastUtils.SetData(TempData, "Invalid login attempt", true);
            return RedirectToAction("Index", "About");
        }

        [HttpGet]
        public IActionResult CreatePassword(LoginVM loginVM)
        {
            return View(loginVM);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(LoginVM loginVM)
        {
            //TODO: replace with generated;
            var newPassword = "newPassword";
            var passwordHash = AccountUtils.ConvertToMD5(newPassword);
            var user = await _documentsRepository.GetAllAsQueryable<User>().FirstAsync(x => x.Email == loginVM.Email);
            user.PasswordHash = AccountUtils.ConvertToMD5(passwordHash);
            await _documentsRepository.UpdateAsync(user);
            await _emailSender.SendEmailAsync(
                toEmail: user.Email,
                subject: "Reset Password",
                message: $"Your new password is: {newPassword}"
            );
            ToastUtils.SetData(TempData, $"Password was reset and email with new password was sent to {user.Email}");
            return RedirectToAction("Index", "About");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePasswordSubmit(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var passwordHash = AccountUtils.ConvertToMD5(loginVM.Password);
                var user = await _documentsRepository.GetAllAsQueryable<User>().FirstAsync(x => x.Email == loginVM.Email);
                user.PasswordHash = AccountUtils.ConvertToMD5(passwordHash);
                await _documentsRepository.UpdateAsync(user);
                return RedirectToAction("Index", "About");
            }
            return View(loginVM);
        }

        [HttpGet]
        public IActionResult LoginWithGoogle()
        {
            var properties = new AuthenticationProperties { RedirectUri = "/" };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "About");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //TODO: make me private;
        public async Task RegisterExternalUser([FromBody] ExternalUserRegistrationDTO data)
        {
            var user = await _documentsRepository.GetAllAsQueryable<User>().FirstOrDefaultAsync(x => x.Email == data.Email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    Email = data.Email,
                    Source = data.Source,
                };
                await _documentsRepository.CreateAsync(user);
            }
        }
    }
}
