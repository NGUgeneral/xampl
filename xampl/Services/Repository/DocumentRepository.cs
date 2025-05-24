using Microsoft.EntityFrameworkCore;
namespace xampl.Services.Repository
{
    public interface IDocumentRepository
    {

    }

    public partial class Repository<TDbContext> : IRepository<TDbContext> where TDbContext : DbContext
    {

    }
}
