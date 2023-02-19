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
        private int MaxResponsesPerTime;


        public GenericRepository(MainDbContext mainDb, IOptions<RepositoryDataSettings> _settings)
        {
            this._mainDb = mainDb;
            this._entities = _mainDb.Set<TEntity>();
            this._settings = _settings;

            this.MaxResponsesPerTime = _settings.Value.MaxResponsesPerTime;

        }

        // return state and count also if count is 0 or not found
        // db error return state.DbError
        public virtual async Task<(ResultState state, int value)> CountQueryAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            try
            {
                nullFilterSolver(ref filter);

                int count = await _entities.CountAsync(filter);
                return await Task.FromResult((ResultState.Seccess, count));

            }
            catch (Exception ex)
            {

                return await Task.FromResult((ResultState.DbError, -1));
            }
        }

        public virtual async Task<RepositoryResult<TEntity>> CreateAsync(TEntity entity)
        {
            try
            {
                var validatoinResult = RepositoryValidationResult.DataAnnotationsValidation(entity);
                if (!validatoinResult.IsValid)
                {
                    return RepositoryResult<TEntity>.FailureValidationResult(validatoinResult.ValidationErrors);
                }
                _entities.Add(entity);

                await _mainDb.SaveChangesAsync();

                return RepositoryResult<TEntity>.SuccessResult(state: ResultState.Created, entity);

            }
            catch (Exception ex)
            {
                return RepositoryResult<TEntity>.FailureResult(ResultState.DbError, null, new List<string> { ex.Message });
            }
        }

        public virtual Task<RepositoryResult<TEntity>> CreateRangeAsync(List<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public async Task<RepositoryResult<TEntity>> CreateUniqeByAsync(TEntity entity, Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                var exsistItem = await _entities.FirstOrDefaultAsync(filter);
                if (exsistItem is not null)
                {
                    return RepositoryResult<TEntity>.FailureResult(ResultState.ItemAlreadyExsist, null);
                }

                return await CreateAsync(entity);

            }
            catch (Exception ex)
            {
                return RepositoryResult<TEntity>.FailureResult(ResultState.DbError, null, new List<string> { ex.Message });
            }
        }

        public virtual Task<RepositoryResult<TEntity>> DeleteAsync(TKey id)
        {
            throw new NotImplementedException();
        }

        public virtual Task<RepositoryResult<TEntity>> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            nullFilterSolver(ref filter);
            throw new NotImplementedException();
        }

        public virtual Task<RepositoryResult<TEntity>> GetByIdAsync(TKey id)
        {
            throw new NotImplementedException();
        }

        public virtual Task<RepositoryValidationResult> IsValid(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task<RepositoryValidationResult> IsValid(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public virtual Task<RepositoryResult<List<TEntity>>> QueryAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            nullFilterSolver(ref filter);

            throw new NotImplementedException();
        }

        public virtual async Task<RepositoryResult<List<TEntity>>> QueryInRangeAsync(int pageIndex, Expression<Func<TEntity, bool>> filter = null)
        {
            try
            {
                nullFilterSolver(ref filter);
                var countResult = await this.CountQueryAsync(filter);

                if (countResult.state != ResultState.Seccess) {
                    return RepositoryResult<List<TEntity>>.FailureResult(countResult.state, null);
                }
                if (countResult.value <= 0) {
                    return RepositoryResult<List<TEntity>>.FailureResult(ResultState.MultipleNotFound, null);

                }

                List<TEntity> entities = await _entities.Where(filter).
                    Skip((pageIndex - 1) * MaxResponsesPerTime).Take(MaxResponsesPerTime).ToListAsync();

                return RepositoryResult<List<TEntity>>.SuccessResult(state: ResultState.Seccess, entities);

            }
            catch (Exception ex)
            {
                return RepositoryResult<List<TEntity>>.FailureResult(ResultState.DbError, null, new List<string> { ex.Message });

            }
        }


        public  virtual Task<RepositoryResult<TEntity>> UpdateAsync(TKey id, TEntity entity)
        {
            throw new NotImplementedException();
        }

        public virtual Expression<Func<TEntity, bool>> DefaultFilter()
        {
            return entity => true;
        }
        public virtual void nullFilterSolver(ref Expression<Func<TEntity, bool>> filter) {
            if (filter == null)
            {
                filter = DefaultFilter();
            }
        }

        
    }
}
