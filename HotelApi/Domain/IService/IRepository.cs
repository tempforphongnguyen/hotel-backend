using Infrastructure.Entities;
using Infrastructure.ViewModel;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService
{
    public interface IRepository<TEntity>
    {
        void Add(TEntity entity);
        Task AddListAsync(IList<TEntity> entities);

        void Remove(TEntity entity);

        void Update(TEntity entity);
        void Update(IList<TEntity> entity);

        int GetTotalEntity(Expression<Func<TEntity, bool>> predicate = null);
        Task<bool> CheckHasAnyEntityWithCondition(Expression<Func<TEntity, bool>> condition = null);
        Task<decimal>ReportOrder(Expression<Func<TEntity, bool>> predicate = null, string totalParam = "TotalPrice");

        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null,Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool orderDesByCreate = false);
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool tracking = true);
        Task<IEnumerable<TEntity>> GetWithPredicateAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool orderDesByCreate = false);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null);
        Task<IEnumerable<TEntity>> GetWithPredicateAndPagingListAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, int pageNumber = 0, int pageSize = 1, bool orderDesByCreate = false);
        Task<IEnumerable<TEntity>> GetWithPredicateAndPagingListWithOrderPropAsync(PagingListPropModel pagingListPropModel, Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool tracking = true);
    }
}
