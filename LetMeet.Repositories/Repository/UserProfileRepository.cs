using LetMeet.Data;
using LetMeet.Data.Entites.UsersInfo;
using LetMeet.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories.Repository
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly MainDbContext _mainDb;
        private readonly DbSet<UserInfo> _usersInfo;
        private readonly RepositoryDataSettings _settings;
        private readonly IGenericRepository<UserInfo, Guid> _genericUserRepository;
        public UserProfileRepository(MainDbContext mainDb, IOptions<RepositoryDataSettings> repoSettingsOptions, IGenericRepository<UserInfo, Guid> genericUserRepository)
        {
            _mainDb = mainDb;
            _usersInfo = mainDb.Set<UserInfo>();
            _settings = repoSettingsOptions.Value;
            _genericUserRepository = genericUserRepository;
        }


        public async Task<RepositoryResult<UserInfo>> GetUserByIdAsync(Guid userInfoId)
        {
            return await _genericUserRepository.GetByIdAsync(userInfoId);
        }

        public Task<RepositoryResult<UserInfo>> GetUserIdAsync(Expression<Func<UserInfo, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public Task<RepositoryResult<UserInfo>> GetUserWhereAsync(Expression<Func<UserInfo, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public virtual Expression<Func<UserInfo, bool>> DefaultFilter()
        {
            return entity => true;
        }

        public virtual void nullFilterSolver(ref Expression<Func<UserInfo, bool>> filter)
        {
            if (filter == null)
            {
                filter = DefaultFilter();
            }
        }

        public async Task<(ResultState state, Guid? value)> GetIdentityIdAsync(Guid userInfoId)
        {
            try
            {
                //nullFilterSolver(ref filter);

                Guid? userIdentityId = await _usersInfo.Where(u => u.id == userInfoId).Select(u => u.identityId).FirstOrDefaultAsync();

                if (userIdentityId == null || userIdentityId.Equals(Guid.Empty))
                {
                    return await Task.FromResult((ResultState.NotFound,Guid.Empty));
                }

                return await Task.FromResult((ResultState.Seccess, userIdentityId));


            }
            catch (Exception ex)
            {
                return await Task.FromResult((ResultState.DbError, Guid.Empty));

            }
        }
    }
}
