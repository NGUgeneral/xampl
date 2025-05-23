using Microsoft.EntityFrameworkCore;

namespace xampl.Services.Repository
{
    public partial class Repository<TDbContext> : IRepository<TDbContext> where TDbContext : DbContext
    {
        protected TDbContext dbContext;

        public Repository(TDbContext context)
        {
            this.dbContext = context;
        }

        public async Task CreateAsync<T>(T entity) where T : class
        {
            dbContext.Set<T>().Add(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task CreateManyAsync<T>(IEnumerable<T> entites) where T : class
        {
            foreach (T entity in entites)
            {
                dbContext.Set<T>().Add(entity);
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync<T>(T entity) where T : class
        {
            dbContext.Set<T>().Remove(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<T>> FindAll<T>() where T : class
        {
            return await dbContext.Set<T>().ToListAsync();
        }

        public async Task<T?> FindById<T>(int id) where T : class
        {
            return await dbContext.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync<T>(T entity) where T : class
        {
            dbContext.Set<T>().Update(entity);
            await dbContext.SaveChangesAsync();
        }

        public IQueryable<T> GetAllAsQueryable<T>() where T : class
        {
            return dbContext.Set<T>().AsQueryable();
		}

        public void Save()
        {
            dbContext.SaveChanges();
        }
    }
}
