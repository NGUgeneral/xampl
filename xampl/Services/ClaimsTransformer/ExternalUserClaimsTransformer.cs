using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using xampl.Models.Documents;
using xampl.Services.RepositoryService;

namespace xampl.Services.ClaimsTransformer
{
    public class ExternalUserClaimsTransformer(
        IRepository<DocumentsContext> documentsRepository
    ) : IClaimsTransformation
    {
        private readonly IRepository<DocumentsContext> _documentsRepository = documentsRepository;

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (!principal.HasClaim(c => c.Type == ClaimTypes.Role))
            {
                var externalId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = principal.FindFirst(ClaimTypes.Email)?.Value;

                if (!string.IsNullOrEmpty(email))
                {
                    var user = await _documentsRepository.GetAllAsQueryable<User>()
                                       .Include(u => u.UserRoles)
                                       .ThenInclude(ur => ur.Role)
                                       .FirstOrDefaultAsync(x => x.Email == email);

                    if (user != null)
                    {
                        var claimsIdentity = (ClaimsIdentity)principal.Identity!;
                        foreach (var userRole in user.UserRoles)
                        {
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, userRole.Role.Title));
                        }
                    }
                }
            }

            return principal;
        }
    }
}
