using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using xampl.Models.Documents;
using xampl.Models.DTO;
using xampl.Services.Repository;
using xampl.Utils;
using xampl.ViewModels;

namespace xampl.Controllers
{
    public class AccountController(
        IRepository<DocumentsContext> documentsRepository
    ) : Controller
    {
        private readonly IRepository<DocumentsContext> _documentsRepository = documentsRepository;

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            var passwordHash = ConvertToMD5(loginVM.Password);
            var user = await _documentsRepository.GetAllAsQueryable<User>().FirstOrDefaultAsync(x => x.Email == loginVM.Email);
            if (user != null && user.PasswordHash == passwordHash)
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

        private static string ConvertToMD5(string input)
        {
            //TODO: move this to utils;
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = MD5.HashData(inputBytes);
            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
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
