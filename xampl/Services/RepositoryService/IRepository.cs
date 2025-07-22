namespace xampl.Services.RepositoryService
{
    public interface IRepository<TContext> :
        IDocumentRepository
    {
        Task<List<T>> FindAll<T>() where T : class;
        Task<T?> FindById<T>(int id) where T : class;
        Task CreateAsync<T>(T entity) where T : class;
        Task CreateManyAsync<T>(IEnumerable<T> entites) where T : class;
        Task UpdateAsync<T>(T entity) where T : class;
        Task DeleteAsync<T>(T entity) where T : class;
        Task DeleteManyAsync<T>(IEnumerable<T> entites) where T : class;
        IQueryable<T> GetAllAsQueryable<T>() where T : class;
        void Save();
    }
}
