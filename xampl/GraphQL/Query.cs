using Microsoft.EntityFrameworkCore;
using xampl.Models.Xampl;
using xampl.Services.RepositoryService;

namespace xampl.GraphQL
{
    public class Query
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Document> GetDocuments([Service] IRepository<XamplContext> repo)
        {
            return repo.GetAllAsQueryable<Document>();
        }

        public async Task<Document?> GetDocumentById([Service] IRepository<XamplContext> repo, int documentId)
        {
            return await repo.GetAllAsQueryable<Document>().FirstOrDefaultAsync(x => x.Id == documentId);
        }
    }
}
