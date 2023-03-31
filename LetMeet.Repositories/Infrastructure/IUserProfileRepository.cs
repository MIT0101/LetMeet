
using LetMeet.Data.Entites.UsersInfo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories.Infrastructure
{
    public interface IUserProfileRepository 
    {
        Task<RepositoryResult<UserInfo>> GetUserWhereAsync(Expression<Func<UserInfo, bool>> filter);

        Task<RepositoryResult<UserInfo>> GetUserByIdAsync(Guid userInfoId);

        Task<RepositoryResult<UserInfo>> GetUserIdAsync(Expression<Func<UserInfo, bool>> filter);

        Task<RepositoryResult<string>> UpdateProfileImageAsync(Guid userInfoId, MemoryStream imageStream, string folderPath);


        Task<(ResultState state, Guid? value)> GetIdentityIdAsync(Guid userInfoId);

        Task<RepositoryResult<SupervisionInfo>> UpdateSupervison(UserInfo supervisor,UserInfo student);




    }
}
