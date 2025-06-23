using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SnapLink_Repository.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        // Get all entities
        Task<IEnumerable<T>> GetAllAsync();
        
        // Get entity by ID
        Task<T> GetByIdAsync(object id);
        
        // Get entities with filter
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter = null, 
                                     Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
                                     string includeProperties = "");
        
        // Add entity
        Task<T> AddAsync(T entity);
        
        // Add range of entities
        Task AddRangeAsync(IEnumerable<T> entities);
        
        // Update entity
        void Update(T entity);
        
        // Update range of entities
        void UpdateRange(IEnumerable<T> entities);
        
        // Remove entity
        void Remove(T entity);
        
        // Remove range of entities
        void RemoveRange(IEnumerable<T> entities);
        
        // Check if entity exists
        Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
        
        // Count entities
        Task<int> CountAsync(Expression<Func<T, bool>> filter = null);
    }
}
