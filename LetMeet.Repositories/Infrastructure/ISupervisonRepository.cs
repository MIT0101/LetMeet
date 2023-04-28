using LetMeet.Data.Dtos.Supervision;
using LetMeet.Data.Entites.UsersInfo;

namespace LetMeet.Repositories.Infrastructure
{
    public interface ISupervisonRepository
    {
        Task<RepositoryResult<SupervisionInfo>> GetSupervisionAsync(UserInfo supervisor, UserInfo student);
        Task<RepositoryResult<SupervisionInfo>> GetSupervisionAsync(Guid studentId);

        Task<RepositoryResult<SupervisionInfo>> CreateAsync(SupervisionInfo supervisionInfo);
        Task<RepositoryResult<SupervisionInfo>> UpdateAsync(SupervisionInfo supervisionInfo);

        Task<RepositoryResult<SupervisionInfo>> RemoveAsync(SupervisionInfo supervisionInfo);

        Task<(ResultState state, int value)> GetCurrentSupervisorStudents(UserInfo supervisor);

        Task<RepositoryResult<List<SupervisorOrStudentSelectDto>>> GetAvailableSupervisorNamesAsync(int maxStudentsPerSupervisor);

        Task<RepositoryResult<SupervisorOrStudentSelectDto>> GetStudentSupervisor(Guid studentInfoId);

        Task<RepositoryResult<IEnumerable<StudentDatedSelectDto>>> GetSupervisorStudents(Guid supervisorId);


        Task<RepositoryResult<IEnumerable<StudentSelectDto>>> GetUnSupervisedStudents();

        //Task<RepositoryResult<SupervisionInfo>> GetSupervision_Meetings_FreeDaysAsync(Guid supervisorId, Guid studentId);

        Task<RepositoryResult<SupervisionInfo>> GetSupervisionAsync(Guid supervisorId, Guid studentId);








    }
}