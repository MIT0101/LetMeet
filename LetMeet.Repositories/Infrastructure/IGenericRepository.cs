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
        //or db error
        Task<RepositoryResult<TEntity>> CreateAsync(TEntity entity);

        //when secsess return State.Created
        //when validtion return validation error
        //when item exsist return item alrady exsist db error
        Task<RepositoryResult<TEntity>> CreateUniqeByAsync(TEntity entity, Expression<Func<TEntity, bool>> filter);
        Task<RepositoryResult<TEntity>> GetByIdAsync(TKey id);

        Task<RepositoryResult<IQueryable<TEntity>>> QueryAsync(Expression<Func<TEntity, bool>> filter = null);

        Task<RepositoryResult<TEntity>> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null);


        Task<RepositoryResult<TEntity>> UpdateAsync(TKey id,TEntity entity);

        Task<RepositoryResult<TEntity>> DeleteAsync(TKey id);

        Task<RepositoryValidationResult> IsValid(TEntity entity);
        Task<RepositoryValidationResult> IsValid(IEnumerable<TEntity> entities);


    }
}
