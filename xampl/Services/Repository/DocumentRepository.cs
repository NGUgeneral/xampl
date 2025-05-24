using Microsoft.EntityFrameworkCore;
using xampl.Models.Documents;
namespace xampl.Services.Repository
{
    public interface IDocumentRepository
    {
        public Task<Document?> GetDocumentById(int documentId);
    }

    public partial class Repository<TDbContext> : IRepository<TDbContext> where TDbContext : DbContext
    {
        public async Task<Document?> GetDocumentById(int documentId)
        {
            return await dbContext.Set<Document>()
                .Where(d => d.Id == documentId)
                .Include(d => d.DocumentNotes)
                .Include(d => d.DocumentLists)
                .ThenInclude(dl => dl.DocumentListItems)
                .FirstOrDefaultAsync();
        }
    }
}
