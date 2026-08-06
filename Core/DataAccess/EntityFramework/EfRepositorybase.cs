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

        protected readonly TContext _context;

        public EfRepositorybase(TContext context)
        {
            _context = context;
        }
        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();

        }

        public async Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
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
            
            if (tracking)
            {
                if (expression == null)
                    return await _context.Set<TEntity>().FirstOrDefaultAsync();

                return await _context.Set<TEntity>().FirstOrDefaultAsync(expression);
            }

            if (expression == null)
                return await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync();

            return await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(expression);
        }
        public async Task<TEntity?> GetByIdAsync(Guid id)
        {
            using TContext context = new();
            return await context.Set<TEntity>().FindAsync(id);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
