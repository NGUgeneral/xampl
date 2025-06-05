using Newtonsoft.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using xampl.Models.DTO;

namespace xampl.Utils
{
    public class AccountUtils
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
                var domain = ConfigUtils.GetSetting("Variables:Domain");
                var response = await httpClient.PostAsync(
                    $"https://{domain}/Account/RegisterExternalUser",
                    new StringContent(dataJson, Encoding.UTF8, "application/json")
                );
            }
        }

        public static string ConvertToMD5(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = MD5.HashData(inputBytes);
            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
