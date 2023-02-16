using LetMeet.Data;
using LetMeet.Data.Entites.UsersInfo;
using LetMeet.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories.Repository
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> 
        where TEntity : class 
        where TKey : IEquatable<TKey>
    {
        private readonly IOptions<RepositoryDataSettings> _settings;
        private readonly MainDbContext _mainDb;
        private readonly DbSet<TEntity> _entities;


        public GenericRepository(MainDbContext mainDb, IOptions<RepositoryDataSettings> _settings)
        {
            this._mainDb = mainDb;
            this._entities = _mainDb.Set<TEntity>();
            this._settings = _settings;
           
           
        }
        public virtual Task<RepositoryResult<TEntity>> CreateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task<RepositoryResult<TEntity>> DeleteAsync(TKey id)
        {
            throw new NotImplementedException();
        }

        public virtual Task<RepositoryResult<TEntity>> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task<RepositoryResult<TEntity>> GetByIdAsync(TKey id)
        {
            throw new NotImplementedException();
        }

        public Task<RepositoryValidationResult> IsValid(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<RepositoryValidationResult> IsValid(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public virtual Task<RepositoryResult<IQueryable<TEntity>>> QueryAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task<RepositoryResult<TEntity>> UpdateAsync(TKey id, TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
