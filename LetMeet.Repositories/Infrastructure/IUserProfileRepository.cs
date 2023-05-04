
using LetMeet.Data.Dtos.Supervision;
using LetMeet.Data.Dtos.User;
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
        Task<RepositoryResult<UserInfo>> GetUserByIdAsync(Guid userInfoId);

        Task<RepositoryResult<string>> UpdateProfileImageAsync(Guid userInfoId, MemoryStream imageStream, string folderPath);


        Task<(ResultState state, Guid? value)> GetIdentityIdAsync(Guid userInfoId);

        Task<RepositoryResult<DayFree>> AddFreeDay(Guid userinfoId, AddFreeDayDto freeDayDto);
        Task<RepositoryResult<DayFree>> RemoveFreeDay(Guid userinfoId, int freeDayId);
        Task<RepositoryResult<List<DayFree>>> GetFreeDaysAsync(Guid userinfoId);

        Task<RepositoryResult<StudentSelectDto>> GetSummary(Guid userinfoId);

        Task<RepositoryResult<StudentProfileDto>> GetStudentProfile (Guid studentId);

        Task<RepositoryResult<SupervisorProfileDto>> GetSupervisorProfile(Guid supervisorId);






    }
}
