using DonationHub.Application.Interfaces;
using DonationHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace DonationHub.Infrastructure.Services
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly AppDbContext dbContext;

        public GenericRepository(AppDbContext dbContext )
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
            => await dbContext.Set<TEntity>().AsNoTracking().ToListAsync();

        public async Task<TEntity?> GetByIdAsyncWithoutTracking(Expression<Func<TEntity, bool>> predicate,
                                                               CancellationToken cancellationToken = default)
           => await dbContext.Set<TEntity>().AsQueryable().AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

        public async Task<TResult?> GetByIdAsyncWithTracking<TResult>(Expression<Func<TEntity, bool>> predicate,
                                                            Expression<Func<TEntity,TResult>> selector, 
                                                            CancellationToken cancellationToken = default,
                                                            params Expression<Func<TEntity, object?>>[]? includes)
        {
            IQueryable<TEntity> query = dbContext.Set<TEntity>();
            if(includes is { Length: > 0 })
            {
                foreach(var item in includes)
                {
                    query = query.Include(item);
                }
            }
            query = query.Where(predicate);

            //. this expression will be existed in the Change Tracker container
            return await query.Select(selector).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TEntity?> GetTrackedAsync(Expression<Func<TEntity, bool>> predicate,
                                                    bool ignoreQueryFilters = false,
                                                    CancellationToken cancellationToken = default,
                                                    params Expression<Func<TEntity, object?>>[]? includes)
        {
            IQueryable<TEntity> query = dbContext.Set<TEntity>();

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            if (includes is { Length: > 0 })
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate,
                                          CancellationToken cancellationToken = default)
            => await dbContext.Set<TEntity>().CountAsync(predicate, cancellationToken);

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) 
                            => await dbContext.Set<TEntity>().AddAsync(entity);
        public async Task EditAsync(TEntity entity, CancellationToken cancellationToken = default) 
                            => dbContext.Set<TEntity>().Update(entity);

        public async Task<int> ExecuteEditAsync(Expression<Func<TEntity, bool>> predicate,
                                                Action<UpdateSettersBuilder<TEntity>> setters,
                                                CancellationToken cancellationToken = default)
            => await dbContext.Set<TEntity>().Where(predicate).ExecuteUpdateAsync(setters, cancellationToken);

        public async Task<(IEnumerable<TResult> Items, int TotalCount)> GetPagedAsync<TResult>(
                                              int page,
                                              int pageSize,
                                              Expression<Func<TEntity, bool>>? predicate = null,
                                              Expression<Func<TEntity, TResult>>? selector = null,
                                              string? orderBy = null,
                                              bool orderByDescending = false,
                                              string? searchTerm = null,
                                              CancellationToken cancellationToken = default,
                                              params Expression<Func<TEntity, object?>>[]? includes) where TResult : class
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector), "Selector must be provided for paged queries.");

            IQueryable<TEntity> query = dbContext.Set<TEntity>();
            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include).AsSplitQuery();
                }
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Basic search: try to apply search against any string property -- best-effort
                var parameter = Expression.Parameter(typeof(TEntity), "x");
                Expression? searchExpr = null;
                foreach (var prop in typeof(TEntity).GetProperties())
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        MemberExpression member = Expression.Property(parameter, prop);
                        //. The Array must be existed here  (don't  forget) 
                        //. Try to create function like the Contains function that existing in string functions 
                        //. and we will search about the SearchTerm that existing as parameter
                        MethodCallExpression contains =
                         Expression.Call(member, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, Expression.Constant(searchTerm));
                        searchExpr = searchExpr == null ? contains : Expression.OrElse(searchExpr, contains);
                    }
                }

                if (searchExpr != null)
                {
                    var lambda = Expression.Lambda<Func<TEntity, bool>>(searchExpr, parameter);
                    query = query.Where(lambda);
                }
            }

            if (predicate != null)
                query = query.Where(predicate);

            var totalCount = await query.CountAsync(cancellationToken);

            // Apply ordering if provided
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                var prop = typeof(TEntity).GetProperty(orderBy);
                if (prop != null)
                {
                    //.1-using Expression tree for creating a new paramter with the smae type of the entity
                    //.2- if the boolean value is still false call the function Order by descending if not call normal order by function
                    //.3- the function will be called from the Querable built in class 
                    var param = Expression.Parameter(typeof(TEntity), "x");
                    var body = Expression.Property(param, prop);
                    var lambda = Expression.Lambda(body, param);
                    var methodName = orderByDescending ? "OrderByDescending" : "OrderBy";
                    var resultExpr = Expression.Call(typeof(Queryable), methodName,
                                                     new Type[] { typeof(TEntity), prop.PropertyType },
                                                     query.Expression, Expression.Quote(lambda));
                    query = query.Provider.CreateQuery<TEntity>(resultExpr);
                }
            }

            //. if greater than 0 take it and deal with 
            if (pageSize > 0)
            {
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }

            //. if not ignore it 
            var projected = query.AsNoTracking().Select(selector);
            var items = await projected.ToListAsync(cancellationToken);
            return (items, totalCount);
        }
    }
}
