using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories.Infrastructure
{
    public interface IGenericRepository<TEntity,TKey>  
        where TEntity  : class 
        where TKey : IEquatable<TKey>
    {
        //when secsess return State.Created
        //when validtion return validation error
        //or db error return DbError
        Task<RepositoryResult<TEntity>> CreateAsync(TEntity entity);

        Task<RepositoryResult<TEntity>> CreateRangeAsync(List<TEntity> entities);


        //when secsess return State.Created
        //when validtion return validation error
        //when item exsist return item alrady exsist db error
        Task<RepositoryResult<TEntity>> CreateUniqeByAsync(TEntity entity, Expression<Func<TEntity, bool>> filter);
        Task<RepositoryResult<TEntity>> GetByIdAsync(TKey id);

        //when secsess return State.Seccess
        //when no result return State.NotFound
        //or db error return DbError
        Task<RepositoryResult<List<TEntity>>> QueryAsync(Expression<Func<TEntity, bool>> filter = null);
        //when secsess return State.Seccess
        //when no result at all return State.MultipleNotFound
        //or db error return DbError

        Task<RepositoryResult<List<TEntity>>> QueryInRangeAsync(int pageIndex, Expression<Func<TEntity, bool>> filter = null);


        Task<RepositoryResult<TEntity>> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null);


        Task<RepositoryResult<TEntity>> UpdateAsync(TKey id,TEntity entity);

        Task<RepositoryResult<TEntity>> DeleteAsync(TKey id);

        Task<RepositoryValidationResult> IsValid(TEntity entity);
        Task<RepositoryValidationResult> IsValid(IEnumerable<TEntity> entities);

        Task<(ResultState state, int value)> CountQueryAsync(Expression<Func<TEntity, bool>> filter = null);

        Expression<Func<TEntity, bool>> DefaultFilter();


    }
}
