
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace DonationHub.Application.Interfaces
{
    public interface IGenericRepository<TEntity> 
    {
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        
        Task<TEntity?> GetByIdAsyncWithoutTracking(Expression<Func<TEntity, bool>> predicate,
                                                  CancellationToken cancellationToken = default);
        
        Task<TResult?> GetByIdAsyncWithTracking<TResult>(Expression<Func<TEntity, bool>> predicate,
                                               Expression<Func<TEntity, TResult>> selector,
                                               CancellationToken cancellationToken = default,
                                               params Expression<Func<TEntity, object?>>[]? includes);
        
        Task<TEntity?> GetTrackedAsync(Expression<Func<TEntity, bool>> predicate,
                                       bool ignoreQueryFilters = false,
                                       CancellationToken cancellationToken = default,
                                       params Expression<Func<TEntity, object?>>[]? includes);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate,
                             CancellationToken cancellationToken = default);

        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task EditAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<int> ExecuteEditAsync(Expression<Func<TEntity, bool>> predicate,
                                   Action<UpdateSettersBuilder<TEntity>> setters,
                                   CancellationToken cancellationToken = default);
        Task<(IEnumerable<TResult> Items, int TotalCount)> GetPagedAsync<TResult>(
                                              int page,
                                              int pageSize,
                                              Expression<Func<TEntity, bool>>? predicate = null,
                                              Expression<Func<TEntity, TResult>>? selector = null,
                                              string? orderBy = null,
                                              bool orderByDescending = false,
                                              string? searchTerm = null,
                                              CancellationToken cancellationToken = default,
                                              params Expression<Func<TEntity, object?>>[]? includes) where TResult : class;
    }
}
