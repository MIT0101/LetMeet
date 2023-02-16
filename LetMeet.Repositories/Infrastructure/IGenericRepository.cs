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
        Task<RepositoryResult<TEntity>> CreateAsync(TEntity entity);
        Task<RepositoryResult<TEntity>> GetByIdAsync(TKey id);

        Task<RepositoryResult<IQueryable<TEntity>>> QueryAsync(Expression<Func<TEntity, bool>> filter = null);

        Task<RepositoryResult<TEntity>> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null);


        Task<RepositoryResult<TEntity>> UpdateAsync(TKey id,TEntity entity);

        Task<RepositoryResult<TEntity>> DeleteAsync(TKey id);

        Task<RepositoryValidationResult> IsValid(TEntity entity);
        Task<RepositoryValidationResult> IsValid(IEnumerable<TEntity> entities);


    }
}
