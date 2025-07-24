using Microsoft.EntityFrameworkCore;
using xampl.Models.Xampl;
namespace xampl.Services.RepositoryService
{
    public interface IDocumentRepository
    {
        public Task UpdateUserWithRoles(User user);
    }

    public partial class Repository<TDbContext> : IRepository<TDbContext> where TDbContext : DbContext
    {
        public async Task UpdateUserWithRoles(User user)
        {
            var existingUser = await dbContext.Set<User>()
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == user.Id)
                ?? throw new InvalidOperationException($"User with Id {user.Id} not found.");
            dbContext.Entry(existingUser).CurrentValues.SetValues(user);
            var newRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToHashSet();

            foreach (var existingUserRole in existingUser.UserRoles.ToList())
            {
                if (!newRoleIds.Contains(existingUserRole.RoleId))
                {
                    dbContext.Set<UserRole>().Remove(existingUserRole);
                }
            }

            var existingRoleIds = existingUser.UserRoles.Select(ur => ur.RoleId).ToHashSet();
            foreach (var newUserRole in user.UserRoles)
            {
                if (!existingRoleIds.Contains(newUserRole.RoleId))
                {
                    existingUser.UserRoles.Add(new UserRole { UserId = existingUser.Id, RoleId = newUserRole.RoleId });
                }
            }
            await dbContext.SaveChangesAsync();
        }
    }
}
