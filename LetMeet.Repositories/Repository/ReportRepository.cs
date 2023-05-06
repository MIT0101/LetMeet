using LetMeet.Data;
using LetMeet.Data.Dtos.Reports;
using LetMeet.Data.Entites.Identity;
using LetMeet.Data.Entites.UsersInfo;
using LetMeet.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LetMeet.Repositories.Repository;

public class ReportRepository : IReportRepository
{
    private readonly MainDbContext _mainDb;
    private readonly ILogger<ReportRepository> _logger;
    private readonly AppTimeProvider _appTimeProvider;
    private ISupervisonRepository _supervisonRepo;

    public ReportRepository(ILogger<ReportRepository> logger, MainDbContext mainDb, AppTimeProvider appTimeProvider, ISupervisonRepository supervisonRepo)
    {
        _logger = logger;
        _mainDb = mainDb;
        _appTimeProvider = appTimeProvider;
        this._supervisonRepo = supervisonRepo;
    }

    public async Task<RepositoryResult<List<FullSupervisor>>> GetFullSupervisors(int maxStudentsPersupervisor)
    {
        try
        {
            var fullSupersiprs = await _mainDb.SupervisionInfo.Where(s => s.endDate > _appTimeProvider.Now).GroupBy(s => s.supervisor.id)
                .Where(g => g.Count() >= maxStudentsPersupervisor).Select(x => new FullSupervisor
                {
                    id = x.Key,
                    fullName = x.Select(s => s.supervisor.fullName).FirstOrDefault(),
                    email = x.Select(s => s.supervisor.emailAddress).FirstOrDefault(),
                    currentActiveStudentsCount = x.Count()                  
                }).ToListAsync();

            if (fullSupersiprs is null)
            {
                return RepositoryResult<List<FullSupervisor>>.FailureResult(ResultState.NotFound, null, new List<string> { "No Full Supervisors Found" });
            }

            return RepositoryResult<List<FullSupervisor>>.SuccessResult(ResultState.Seccess, fullSupersiprs);
        }
        catch (Exception ex)
        {
            _logger.LogError("Db Error , {0}", ex);
            return RepositoryResult<List<FullSupervisor>>.FailureResult(ResultState.DbError, null, new List<string> { RepositoryErrors.DbError });

        }
    }

    public async Task<RepositoryResult<List<IdelSupervisor>>> GetIdleSupervisors()
    {
        try
        {
            var idleSupervisors = await _mainDb.UserInfos.Where(u => u.userRole == UserRole.Supervisor)
                .Where(s => _mainDb.SupervisionInfo.Where(sp => sp.supervisor.id == s.id && sp.endDate > _appTimeProvider.Now).Count() == 0)
                .Select(x => new IdelSupervisor { id = x.id, fullName = x.fullName, email = x.emailAddress }).ToListAsync();
            if (idleSupervisors is null)
            {
                return RepositoryResult<List<IdelSupervisor>>.FailureResult(ResultState.NotFound, null, new List<string> { "Not Found" });

            }
            return RepositoryResult<List<IdelSupervisor>>.SuccessResult(ResultState.Seccess, idleSupervisors);

        }
        catch (Exception ex)
        {
            _logger.LogError("Db Error , {0}", ex);
            return RepositoryResult<List<IdelSupervisor>>.FailureResult(ResultState.DbError, null, new List<string> { RepositoryErrors.DbError });
        }
    }

    public async Task<RepositoryResult<StudentReport>> GetStudentReport(Guid studentId)
    {
        try
        {
            var studentReport = await _mainDb.SupervisionInfo.Where(s => s.student.id == studentId).Select(x => new StudentReport
            {
                studentName = x.student.fullName,
                stage= (Stage)x.student.stage,
                supervisorName = x.supervisor.fullName,
                supervisorId = x.supervisor.id,
                supervsionEndDate= x.endDate
            }).FirstOrDefaultAsync();
            if (studentReport is null)
            {
                return RepositoryResult<StudentReport>.FailureResult(ResultState.NotFound, null, new List<string> { "Student Not Found" });
            }
            var studentEndedMeetings = await _mainDb.Meetings.Where(m => m.SupervisionInfo.student.id == studentId && m.date < _appTimeProvider.Now)
            .Select(m => new MeetingReportSummary(m.id, m.SupervisionInfo.supervisor.id, m.SupervisionInfo.student.id, m.date, m.startHour, m.endHour
                        , m.tasks, m.tasks.Count, m.SupervisionInfo.student.fullName, m.SupervisionInfo.supervisor.fullName, m.isSupervisorPresent, m.isStudentPresent)).ToListAsync();

            if (studentEndedMeetings is not null)
            {
                studentReport.EndedMeetings = studentEndedMeetings;
            }
            return RepositoryResult<StudentReport>.SuccessResult(ResultState.Seccess, studentReport);

        }
        catch (Exception ex)
        {

            _logger.LogError("Db Error , {0}", ex);
            return RepositoryResult<StudentReport>.FailureResult(ResultState.DbError, null, new List<string> { RepositoryErrors.DbError });
        }
    }

    public async Task<RepositoryResult<SupervisorReport>> GetSupervisorReport(Guid supervisorId)
    {
        try
        {
            var supervisorReport = await _mainDb.UserInfos.Where(x => x.id == supervisorId && x.userRole == UserRole.Supervisor)
                .Select(x => new SupervisorReport { fullName = x.fullName, email = x.emailAddress }).FirstOrDefaultAsync();
            if (supervisorReport == null)
            {
                return RepositoryResult<SupervisorReport>.FailureResult(ResultState.NotFound, null, new List<string> { "Supervisor Not Found" });
            }
            var supervisorStudents = (await _supervisonRepo.GetSupervisorStudents(supervisorId)).Result;
            if (supervisorStudents != null)
            {

                supervisorReport.students = supervisorStudents.ToList();
            }

            var supervisorEndedMeetings = await _mainDb.Meetings.Where(m => m.SupervisionInfo.supervisor.id == supervisorId && m.date < _appTimeProvider.Now)
            .Select(m => new MeetingReportSummary(m.id, m.SupervisionInfo.supervisor.id, m.SupervisionInfo.student.id, m.date, m.startHour, m.endHour
                        , m.tasks, m.tasks.Count, m.SupervisionInfo.student.fullName, m.SupervisionInfo.supervisor.fullName, m.isSupervisorPresent, m.isStudentPresent)).ToListAsync();

            if (supervisorEndedMeetings != null)
            {
                supervisorReport.EndedMeetings = supervisorEndedMeetings;
            }

            return RepositoryResult<SupervisorReport>.SuccessResult(ResultState.Seccess, supervisorReport);
        }
        catch (Exception ex)
        {

            _logger.LogError("Db Error , {0}", ex);
            return RepositoryResult<SupervisorReport>.FailureResult(ResultState.DbError, null, new List<string> { RepositoryErrors.DbError });
        }
    }

    public async Task<RepositoryResult<List<TopStudentAbsence>>> GetTopStudentsAbsence(int topValue)
    {

        try
        {
            var topStudentsAbsence = await _mainDb.Meetings.Where(m => m.isStudentPresent == false && m.date < _appTimeProvider.Now)
                .GroupBy(m => m.SupervisionInfo.student.id)
                .Select(x => new TopStudentAbsence { id = x.Key, fullName = x.FirstOrDefault().SupervisionInfo.student.fullName
                ,email = x.FirstOrDefault().SupervisionInfo.student.emailAddress,stage = (Stage)x.FirstOrDefault().SupervisionInfo.student.stage,
                supervisorName = x.FirstOrDefault().SupervisionInfo.supervisor.fullName,
                supervisorId = x.FirstOrDefault().SupervisionInfo.supervisor.id, absneceTimes = x.Count() })
                .OrderByDescending(x => x.absneceTimes).Take(topValue).ToListAsync();
            if (topStudentsAbsence is null)
            {
                return RepositoryResult<List<TopStudentAbsence>>.FailureResult(ResultState.NotFound, null, new List<string> { "Not Found" });

            }
            return RepositoryResult<List<TopStudentAbsence>>.SuccessResult(ResultState.Seccess, topStudentsAbsence);


        }
        catch (Exception ex)
        {

            _logger.LogError("Db Error , {0}", ex);
            return RepositoryResult<List<TopStudentAbsence>>.FailureResult(ResultState.DbError, null, new List<string> { RepositoryErrors.DbError });
        }
    }

    public async Task<RepositoryResult<List<TopSupervisorAbsence>>> GetTopSupervisorsAbsence(int topValue)
    {
        try
        {
            var topSupervisorsAbsence = await _mainDb.Meetings.Where(m => m.isSupervisorPresent == false && m.date < _appTimeProvider.Now)
                            .GroupBy(m => m.SupervisionInfo.supervisor.id)
                            .Select(x => new TopSupervisorAbsence { id = x.Key, fullName = x.FirstOrDefault().SupervisionInfo.supervisor.fullName,
                            email = x.FirstOrDefault().SupervisionInfo.supervisor.emailAddress,studentId = x.FirstOrDefault().SupervisionInfo.student.id,
                            studentName = x.FirstOrDefault().SupervisionInfo.student.fullName, absenceTimes = x.Count() })
                            .OrderByDescending(x => x.absenceTimes).Take(topValue).ToListAsync();
            if (topSupervisorsAbsence is null)
            {
                return RepositoryResult<List<TopSupervisorAbsence>>.FailureResult(ResultState.NotFound, null, new List<string> { "Not Found" });
            }
            return RepositoryResult<List<TopSupervisorAbsence>>.SuccessResult(ResultState.Seccess, topSupervisorsAbsence);
        }
        catch (Exception ex)
        {

            _logger.LogError("Db Error , {0}", ex);
            return RepositoryResult<List<TopSupervisorAbsence>>.FailureResult(ResultState.DbError, null, new List<string> { RepositoryErrors.DbError });
        }
    }
}

