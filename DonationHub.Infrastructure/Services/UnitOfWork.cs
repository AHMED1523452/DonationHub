using DonationHub.Application.Interfaces;
using DonationHub.Infrastructure.Persistence;

namespace DonationHub.Infrastructure.Services
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext dbContext;
        private Dictionary<Type, object> repositories = new Dictionary<Type, object>();
        public UnitOfWork(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);
            if (!repositories.TryGetValue(type, out var repository))
            {
                repository = new GenericRepository<TEntity>(dbContext);
                repositories.Add(type, repository);
            }

            return (IGenericRepository<TEntity>)repository;
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }

        public async Task<int> SavechangesAsync(CancellationToken cancellationToken = default)
            => await dbContext.SaveChangesAsync();
    }
}
