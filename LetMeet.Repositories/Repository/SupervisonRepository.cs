using LetMeet.Data;
using LetMeet.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetMeet.Data.Entites.UsersInfo;
using System.ComponentModel.DataAnnotations;
using LetMeet.Data.Dtos.Supervision;
using LetMeet.Data.Entites.Identity;

namespace LetMeet.Repositories.Repository
{
    public class SupervisonRepository : ISupervisonRepository
    {
        private readonly ILogger<UserProfileRepository> _logger;

        private readonly MainDbContext _mainDb;
        private readonly DbSet<SupervisionInfo> _supervisionInfo;
        private readonly IGenericRepository<SupervisionInfo, int> _supervionGRepo;
        private readonly RepositoryDataSettings _settings;

        private readonly AppTimeProvider _appTimeProvider;

        public SupervisonRepository(MainDbContext mainDb, IOptions<RepositoryDataSettings> repoSettingsOptions, ILogger<UserProfileRepository> logger, IGenericRepository<SupervisionInfo, int> supervionGRepo, AppTimeProvider appTimeProvider)
        {
            _mainDb = mainDb;
            _supervisionInfo = mainDb.Set<SupervisionInfo>();
            _settings = repoSettingsOptions.Value;
            _logger = logger;
            _supervionGRepo = supervionGRepo;
            _appTimeProvider = appTimeProvider;
        }

        public Task<RepositoryResult<SupervisionInfo>> CreateAsync(SupervisionInfo supervisionInfo)
        {
         return _supervionGRepo.CreateAsync(supervisionInfo);
        }

        public async Task<RepositoryResult<List<SupervisorOrStudentSelectDto>>> GetAvailableSupervisorNamesAsync(int maxStudentsPerSupervisor)
        {
            try
            {
                //get list of UserInfo entity where useres are in Supervisor role and user in SupervisionInfo (endDate bigger or equal _appTimeProvider.Now and) count is 6 or higher 
         
                var res = await _mainDb.UserInfos.Where(u => u.userRole == UserRole.Supervisor &&
                _mainDb.SupervisionInfo.Count(s => s.supervisor==u&&s.endDate >= _appTimeProvider.Now) < maxStudentsPerSupervisor).
                Select(u => new SupervisorOrStudentSelectDto(u.id, u.fullName)).ToListAsync();

                return RepositoryResult<List<SupervisorOrStudentSelectDto>>.SuccessResult(ResultState.Seccess,res);
            }
            catch (Exception ex)
            {

                return RepositoryResult<List<SupervisorOrStudentSelectDto>>.FailureResult(ResultState.DbError, null, new List<string> { ex.Message });

            }
        }

        public async Task<(ResultState state, int value)> GetCurrentSupervisorStudents(UserInfo supervisor)
        {
            return await _supervionGRepo.CountQueryAsync(s=>s.endDate>=_appTimeProvider.Now&&s.supervisor==supervisor);
        }

        public async Task<RepositoryResult<SupervisorOrStudentSelectDto>> GetStudentSupervisor(Guid studentInfoId)
        {
            try
            {
               var result=await _supervisionInfo.Where(s => s.student.id == studentInfoId).Select(s=>new SupervisorOrStudentSelectDto(s.supervisor.id,s.supervisor.fullName)).SingleOrDefaultAsync();
                if (result is null)
                {
                    return RepositoryResult<SupervisorOrStudentSelectDto>.FailureResult(ResultState.NotFound, null);
                }
                return RepositoryResult<SupervisorOrStudentSelectDto>.SuccessResult(ResultState.Seccess, result);
            }
            catch (Exception ex)
            {
                return RepositoryResult<SupervisorOrStudentSelectDto>.FailureResult(ResultState.DbError, null, new List<string> { ex.Message });


            }
        }

        public async Task<RepositoryResult<SupervisionInfo>> GetSupervisionAsync(UserInfo supervisor, UserInfo student)
        {
            try
            {
                if (supervisor is null || student is null)
                {
                    return RepositoryResult<SupervisionInfo>.FailureValidationResult(new List<ValidationResult>() { new ValidationResult("Supervisot or Student is Empity") });

                }

                SupervisionInfo supervisionInfo = await _supervisionInfo.FirstOrDefaultAsync(s => s.supervisor == supervisor && s.student == student);

                if (supervisionInfo is null)
                {

                    return RepositoryResult<SupervisionInfo>.FailureResult(ResultState.NotFound, null);
                }

                return RepositoryResult<SupervisionInfo>.SuccessResult(state: ResultState.Seccess, supervisionInfo);

            }
            catch (Exception ex)
            {
                return RepositoryResult<SupervisionInfo>.FailureResult(ResultState.DbError, null, new List<string> { ex.Message });
            }

        }

        public Task<RepositoryResult<SupervisionInfo>> GetSupervisionAsync(Guid studentId)
        {          
            return _supervionGRepo.FirstOrDefaultAsync(s => s.student.id == studentId);
        }

        public async Task<RepositoryResult<IEnumerable<StudentDatedSelectDto>>> GetSupervisorStudents(Guid supervisorId)
        {
            try
            {
                var res = await  _mainDb.SupervisionInfo.Where(s => s.supervisor.id == supervisorId).
                Select(u => new StudentDatedSelectDto(u.student.id, u.student.fullName,u.endDate)).ToListAsync();

                return RepositoryResult<IEnumerable<StudentDatedSelectDto>>.SuccessResult(ResultState.Seccess, res);
            
            }
            catch (Exception ex)
            {

                return RepositoryResult<IEnumerable<StudentDatedSelectDto>>.FailureResult(ResultState.DbError, null, new List<string> { ex.Message });
            }
        }

        public async Task<RepositoryResult<IEnumerable<StudentSelectDto>>> GetUnSupervisedStudents()
        {
            try
            {
                var res = await _mainDb.UserInfos.Where(u => u.userRole == UserRole.Student &&
                               _mainDb.SupervisionInfo.Count(s =>s.student==u&&s.endDate>=_appTimeProvider.Now) == 0).
                Select(u => new StudentSelectDto(u.id, u.fullName)).ToListAsync();

                return RepositoryResult<IEnumerable<StudentSelectDto>>.SuccessResult(ResultState.Seccess, res);

            }
            catch (Exception ex)
            {

                return RepositoryResult<IEnumerable<StudentSelectDto>>.FailureResult(ResultState.DbError, null, new List<string> { ex.Message });
            }
        }

        public Task<RepositoryResult<SupervisionInfo>> RemoveAsync(SupervisionInfo supervisionInfo)
        {
            return _supervionGRepo.RemoveAsync(supervisionInfo.id);
        }

        public Task<RepositoryResult<SupervisionInfo>> UpdateAsync(SupervisionInfo supervisionInfo)
        {
            return _supervionGRepo.UpdateAsync(supervisionInfo.id,supervisionInfo);
        }

        public Task<RepositoryResult<SupervisionInfo>> GetSupervisionAsync(Guid supervisorId, Guid studentId)
        {
            return _supervionGRepo.FirstOrDefaultAsync(s => s.supervisor.id == supervisorId && s.student.id == studentId);
        }
    }
}
