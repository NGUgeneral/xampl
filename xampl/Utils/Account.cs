using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using xampl.Models.DTO;

namespace xampl.Utils
{
    public class Account
    {
        public static async Task MaybeRegisterExternalUser(ClaimsPrincipal? principal)
        {
            if (principal is not null)
            {
                using var httpClient = new HttpClient();
                var data = new ExternalUserRegistrationDTO
                {
                    FirstName = principal.FindFirstValue(ClaimTypes.GivenName),
                    LastName = principal.FindFirstValue(ClaimTypes.Surname),
                    Email = principal.FindFirstValue(ClaimTypes.Email),
                    Source = principal.Identity?.AuthenticationType
                };
                var dataJson = JsonConvert.SerializeObject(data);
                var response = await httpClient.PostAsync(
                    //TODO: domain from appsettings;
                    "https://localhost:7249/Account/RegisterExternalUser",
                    new StringContent(dataJson, Encoding.UTF8, "application/json")
                );
            }
        }
    }
}
