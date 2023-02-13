using LetMeet.Data;
using LetMeet.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories.Repository
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : class where TKey : IEquatable<TKey>
    {
        private readonly RepositoryDataSettings _settings;
        private readonly MainDbContext _mainDb;
        private readonly DbSet<TEntity> _entities;

        public GenericRepository(MainDbContext mainDb, RepositoryDataSettings _settings)
        {
            this._mainDb = mainDb;
            this._entities = _mainDb.Set<TEntity>();
            _settings = _settings;
        }
        public Task<RepositoryResult<TEntity>> CreateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<RepositoryResult<TEntity>> DeleteAsync(TKey id)
        {
            throw new NotImplementedException();
        }

        public Task<RepositoryResult<TEntity>> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<RepositoryResult<TEntity>> GetByIdAsync(TKey id)
        {
            throw new NotImplementedException();
        }

        public Task<RepositoryResult<IQueryable<TEntity>>> QueryAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public Task<RepositoryResult<TEntity>> UpdateAsync(TKey id, TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
