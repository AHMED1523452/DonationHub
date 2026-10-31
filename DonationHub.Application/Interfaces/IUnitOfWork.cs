
namespace DonationHub.Application.Interfaces
{
    public interface IUnitOfWork
    {
         IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;

         void Dispose();
         Task<int> SavechangesAsync(CancellationToken cancellationToken = default);
    }
}
