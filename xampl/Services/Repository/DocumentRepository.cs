using Microsoft.EntityFrameworkCore;
using xampl.ViewModels;

namespace xampl.Services.Repository
{
    public interface IDocumentRepository
    {
        Task<DocumentVM> GetDocument(int id);
        Task CreateDocument(DocumentVM document);
        Task UpdateDocument(DocumentVM document);
        Task DeleteDocument(int id);

        Task<DocumentListVM> GetDocumentList(int id);
        Task CreateDocumentList(DocumentListVM documentList);
        Task UpdateDocumentList(DocumentListVM documentList);
        Task DeleteDocumentList(int id);
    }

    public partial class Repository<TDbContext> : IRepository<TDbContext> where TDbContext : DbContext
    {
        public async Task<DocumentVM> GetDocument(int id)
        {
            throw new NotImplementedException();
        }

        public async Task CreateDocument(DocumentVM fullDocument)
        {
            throw new NotImplementedException();
        }
        
        public async Task UpdateDocument(DocumentVM fullDocument)
        {
            throw new NotImplementedException();
        }
        
        public async Task DeleteDocument(int id)
        {
            throw new NotImplementedException();
        }
        
        public async Task<DocumentListVM> GetDocumentList(int id)
        {
            throw new NotImplementedException();
        }
        
        public async Task CreateDocumentList(DocumentListVM documentList)
        {
            throw new NotImplementedException();
        }
        
        public async Task UpdateDocumentList(DocumentListVM documentList)
        {
            throw new NotImplementedException();
        }
        
        public async Task DeleteDocumentList(int id)
        {
            throw new NotImplementedException();
        }
    }
}
