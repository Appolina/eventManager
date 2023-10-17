using SQLite;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EventManager.DataAccess
{
    public class SQLiteRepository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        private readonly SQLiteAsyncConnection _connection;

        public SQLiteRepository(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        public async Task<TEntity> GetByIdAsync(object id)
        {
            return await _connection.FindAsync<TEntity>(id);
        }

        public async Task<IReadOnlyCollection<TEntity>> GetAllAsync()
        {
            return await _connection.Table<TEntity>().ToListAsync();
        }

        public async Task<IReadOnlyCollection<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _connection.Table<TEntity>().Where(predicate).ToListAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _connection.InsertAsync(entity);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            await _connection.UpdateAsync(entity);
        }

        public async Task DeleteAsync(TEntity entity)
        {
            await _connection.DeleteAsync(entity);
        }
        
    }
}
