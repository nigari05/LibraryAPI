using Core.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.DataAccess.EntityFramework
{
    public class EfRepositorybase<TEntity, TContext> : IRepositorybase<TEntity>
         where TEntity : class, IEntity
         where TContext : DbContext, new()
    {
        public async Task AddAsync(TEntity entity)
        {
            using TContext context = new();
            var addedEntity = context.Entry(entity);
            addedEntity.State = EntityState.Added;
            await context.SaveChangesAsync();

        }

        public async Task DeleteAsync(TEntity entity)
        {
            using TContext context = new();
            var deletedEntity = context.Entry(entity);
            deletedEntity.State = EntityState.Deleted;
            await context.SaveChangesAsync();
        }
        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? expression = null, bool tracking = true)
        {
            using TContext context = new();
            if (tracking)
                if (expression == null)
                    return await context.Set<TEntity>().ToListAsync();
                else return await context.Set<TEntity>().Where(expression).ToListAsync();
            else
                if (expression == null)
                    return await context.Set<TEntity>().AsNoTracking().ToListAsync();
                else
                    return await context.Set<TEntity>().Where(expression).AsNoTracking().ToListAsync();
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>>? expression = null, bool tracking = true)
        {
            using TContext context = new();
            if (tracking)
            {
                if (expression == null)
                    return await context.Set<TEntity>().FirstOrDefaultAsync();

                return await context.Set<TEntity>().FirstOrDefaultAsync(expression);
            }

            if (expression == null)
                return await context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync();

            return await context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(expression);
        }
        public async Task<TEntity?> GetByIdAsync(Guid id)
        {
            using TContext context = new();
            return await context.Set<TEntity>().FindAsync(id);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            using TContext context = new();
            var updatedEntity = context.Entry(entity);
            updatedEntity.State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
    }
}
