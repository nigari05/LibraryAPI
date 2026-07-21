using Core.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.DataAccess
{
    public interface IRepositorybase<TEntity> where TEntity : class, IEntity
    {
        Task AddAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? expression = null, bool tracking = true);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>>? expression = null, bool tracking = true);
        Task<TEntity?> GetByIdAsync(Guid id);
    }
}
