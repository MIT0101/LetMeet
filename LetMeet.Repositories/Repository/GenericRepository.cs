﻿using Alachisoft.NCache.MapReduce;
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
                return (ResultState.Seccess, count);

            }
            catch (Exception ex)
            {

                return (ResultState.DbError, -1);
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

                return RepositoryResult<TEntity>.SuccessResult(state: ResultState.Seccess, entity);

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

        public async virtual Task<RepositoryResult<TEntity>> RemoveAsync(TKey id)
        {
            try
            {
                var exsistItem = await _entities.FindAsync(id);
                if (exsistItem is null)
                {
                    return RepositoryResult<TEntity>.FailureResult(ResultState.NotFound, null);
                }

                _entities.Remove(exsistItem);

                await _mainDb.SaveChangesAsync();

                return RepositoryResult<TEntity>.SuccessResult(state: ResultState.Seccess, exsistItem);

            }
            catch (Exception ex)
            {
                return RepositoryResult<TEntity>.FailureResult(ResultState.DbError, null, new List<string> { ex.Message });
            }

        }

        public async virtual Task<RepositoryResult<TEntity>> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            try
            {
                nullFilterSolver(ref filter);
                var exsistItem = await _entities.FirstOrDefaultAsync(filter);
                if (exsistItem is null)
                {
                    return RepositoryResult<TEntity>.FailureResult(ResultState.NotFound, null);
                }

                return RepositoryResult<TEntity>.SuccessResult(state: ResultState.Seccess, exsistItem);

            }
            catch (Exception ex)
            {
                return RepositoryResult<TEntity>.FailureResult(ResultState.DbError, null, new List<string> { ex.Message });
            }
        }

        public virtual async Task<RepositoryResult<TEntity>> GetByIdAsync(TKey id)
        {
            try
            {
                if (id is null) {
                    List<ValidationResult> validationErrors = new() {
                    new ValidationResult("Key Is Required")};

                    return RepositoryResult<TEntity>.FailureValidationResult(validationErrors);

                }

                TEntity result = await _entities.FindAsync(id);

                if (result == null) {
                    return RepositoryResult<TEntity>.FailureResult(ResultState.NotFound, validationErrors: null);
                }

                return RepositoryResult<TEntity>.SuccessResult(state: ResultState.Seccess, result);

            }
            catch (Exception ex)
            {
                return RepositoryResult<TEntity>.FailureResult(ResultState.DbError, null, new List<string> { ex.Message });
            }
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
                    return RepositoryResult<List<TEntity>>.FailureResult(ResultState.NotFound, null);

                }

                //List<TEntity> entities = await _entities.Where(filter).
                //    Skip((pageIndex - 1) * MaxResponsesPerTime).Take(MaxResponsesPerTime).ToListAsync();
                 
                //get entities with paging and order by there key
                List<TEntity> entities = await _entities.Where(filter).
                    OrderBy(GetKeySelector()).Skip((pageIndex - 1) * MaxResponsesPerTime).Take(MaxResponsesPerTime).ToListAsync();
 
                return RepositoryResult<List<TEntity>>.SuccessResult(state: ResultState.Seccess, entities);

            }
            catch (Exception ex)
            {
                return RepositoryResult<List<TEntity>>.FailureResult(ResultState.DbError, null, new List<string> { ex.Message });

            }
        }
        //method to get key selector for order by
        private Expression<Func<TEntity, TKey>> GetKeySelector()
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, typeof(TEntity).GetProperty("id"));
            var lambda = Expression.Lambda<Func<TEntity, TKey>>(property, parameter);
            return lambda;
        }


        public  virtual async Task<RepositoryResult<TEntity>> UpdateAsync(TKey id, TEntity entity)
        {
            try
            {
                var validatoinResult = RepositoryValidationResult.DataAnnotationsValidation(entity);
                if (!validatoinResult.IsValid)
                {
                    return RepositoryResult<TEntity>.FailureValidationResult(validatoinResult.ValidationErrors);
                }

                var foudnEnity = _entities.Find(id);

                if (foudnEnity is null) {
                    //not found
                    return RepositoryResult<TEntity>.FailureResult(ResultState.NotFound, null);

                }
                _entities.Update(entity);

                await _mainDb.SaveChangesAsync();

                return RepositoryResult<TEntity>.SuccessResult(state: ResultState.Seccess, entity);


            }
            catch (Exception ex)
            {

                return RepositoryResult<TEntity>.FailureResult(ResultState.DbError, null, new List<string> { ex.Message });
            }
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
