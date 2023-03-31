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

        


    }
}