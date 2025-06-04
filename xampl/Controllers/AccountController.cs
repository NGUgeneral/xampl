using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using xampl.Models.Documents;
using xampl.Services.Repository;
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
            // TODO: Add validation error message and show it in error-toast
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
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
    }
}
