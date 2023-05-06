using LetMeet.Data.Dtos.Reports;
namespace LetMeet.Repositories.Infrastructure;

public interface IReportRepository
{
    Task<RepositoryResult<List<TopStudentAbsence>>> GetTopStudentsAbsence(int topValue);
    Task<RepositoryResult<List<TopSupervisorAbsence>>> GetTopSupervisorsAbsence(int topValue);
    Task<RepositoryResult<List<FullSupervisor>>> GetFullSupervisors(int maxStudentsPersupervisor);
    Task<RepositoryResult<List<IdelSupervisor>>> GetIdleSupervisors();
    Task<RepositoryResult<StudentReport>> GetStudentReport(Guid studentId);
    Task<RepositoryResult<SupervisorReport>> GetSupervisorReport(Guid supervisorId);

}
