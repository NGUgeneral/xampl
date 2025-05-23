using Microsoft.EntityFrameworkCore;
using xampl.Models.ViewModels;

namespace xampl.Services.Repository
{
    public interface IXamplRepository
    {
        Task<DocumentVM> GetFullDocumentById();
        Task CreateFullDocument(DocumentVM fullDocument);
        Task UpdateFullDocument(DocumentVM fullDocument);
        Task DeleteFullDocument(DocumentVM fullDocument);
    }

    public partial class Repository<TDbContext> : IRepository<TDbContext> where TDbContext : DbContext
    {
        public async Task<DocumentVM> GetFullDocumentById()
        {
            throw new NotImplementedException();
        }

        public async Task CreateFullDocument(DocumentVM fullDocument)
        {
            throw new NotImplementedException();
        }
        public async Task UpdateFullDocument(DocumentVM fullDocument)
        {
            throw new NotImplementedException();
        }
        public async Task DeleteFullDocument(DocumentVM fullDocument)
        {
            throw new NotImplementedException();
        }
    }
}
