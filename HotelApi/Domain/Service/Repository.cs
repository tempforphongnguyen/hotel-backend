using Domain.IService;
using Infrastructure.Context;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Infrastructure.ViewModel;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Domain.Service
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IHelperService<TEntity> _helperService;

        public Repository(ApplicationDBContext dbContext, IHelperService<TEntity> helperService)
        {
            _dbContext = dbContext;
            _helperService = helperService;
        }

        public void Add(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
        }

        public async Task AddListAsync(IList<TEntity> entities)
        {
            await _dbContext.Set<TEntity>().AddRangeAsync(entities);
        }

        public void Remove(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }

        public void Update(IList<TEntity> entity)
        {
            _dbContext.Set<TEntity>().UpdateRange(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool orderDesByCreate = false)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();
            if (predicate != null)
                query = query.Where(predicate);

            if (include != null)
                query = include(query);

            query = orderDesByCreate ? query.OrderByDescending(x => x.CreateDate) : query.OrderBy(x => x.CreateDate);

            return await query.ToListAsync().ConfigureAwait(false);
        }

        public async Task<decimal> ReportOrder(Expression<Func<TEntity, bool>> predicate = null, string totalParam = "TotalPrice")
        {
            decimal total = 0;
            IQueryable<Order> query = _dbContext.Set<Order>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var result = await query.GroupBy(s => s.UpdateDate).Select(s => new
            {
                UpdateDate = s.Key,
                Total = s.Sum(s => s.TotalPrice)
            }).ToListAsync();

            total = result.Sum(s => s.Total);
            
            return total;
        }

        public async Task<decimal> ReportOrder(Expression<Func<TEntity, bool>> predicate = null)
        {
            decimal total = 0;
            IQueryable<Order> query = _dbContext.Set<Order>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var result = await query.GroupBy(s => s.UpdateDate).Select(s => new
            {
                UpdateDate = s.Key,
                Total = s.Sum(s => s.TotalPrice)
            }).ToListAsync();

            var temp = result.Select(s => s.UpdateDate);
            total = result.Sum(s => s.Total);

            return total;
        }

        public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool tracking = true)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if(predicate != null)
                query = query.Where(predicate);
            
            if (include != null)
                query = include(query);

            return tracking ? await query.SingleOrDefaultAsync().ConfigureAwait(false) : await query.AsNoTracking().SingleOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<TEntity>> GetWithPredicateAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool orderDesByCreate = false) 
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            query = orderDesByCreate ? query.OrderByDescending(x => x.CreateDate) : query.OrderBy(x => x.CreateDate);

            return await query.ToListAsync().ConfigureAwait(false);
        }
        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (predicate != null)
                query = query.Where(predicate);

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<TEntity>> GetWithPredicateAndPagingListWithOrderPropAsync(PagingListPropModel pagingListPropModel, Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool tracking = true)
        {
            bool isVaildProp = false;

            if (!pagingListPropModel.SortBy.IsNullOrEmpty())
            {
                isVaildProp = _helperService.IsValidProperty(pagingListPropModel.SortBy);
                
                if (!isVaildProp)
                {
                    throw new ArgumentException("Invalid property name", nameof(pagingListPropModel.SortBy));
                }
            }

            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            query = isVaildProp ? pagingListPropModel.SortDesc ? query.OrderBy(pagingListPropModel.SortBy + " descending") : query.OrderBy(pagingListPropModel.SortBy)
                                : query;

            query = query.Skip((pagingListPropModel.PageNumber - 1) * pagingListPropModel.PageSize).Take(pagingListPropModel.PageSize);

            query = tracking ? query : query.AsNoTracking();

            return await query.ToListAsync().ConfigureAwait(false);
        }

        public int GetTotalEntity(Expression<Func<TEntity, bool>> predicate = null)
        {
            var query = _dbContext.Set<TEntity>();
            int total = 0;
            total = predicate is null ? query.Count():query.Count(predicate);

            return total;
        }

        public async Task<bool> CheckHasAnyEntityWithCondition (Expression<Func<TEntity, bool>> condition = null)
        {
            var query = _dbContext.Set<TEntity>();

            var result = condition is null ?await query.AnyAsync() : await query.AnyAsync(condition);
            return result;
        }

        #region Do not use regularly

        public async Task<IEnumerable<TEntity>> GetWithPredicateAndPagingListAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, int pageNumber = 0, int pageSize = 1, bool orderDesByCreate = false)
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            query = orderDesByCreate ? query.OrderByDescending(x => x.CreateDate) : query.OrderBy(x => x.CreateDate);

            if (pageNumber != 0)
            {
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }

            return await query.ToListAsync().ConfigureAwait(false);
        }

        #endregion
    }
}
