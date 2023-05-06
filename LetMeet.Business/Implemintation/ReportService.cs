using LetMeet.Business.Interfaces;
using LetMeet.Business.Results;
using LetMeet.Data.Dtos.Reports;
using LetMeet.Data.Entites.Identity;
using LetMeet.Repositories.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneOf;
using System.ComponentModel.DataAnnotations;

namespace LetMeet.Business.Implemintation;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepo;
    private readonly AppServiceOptions _appServiceOptions;
    private readonly ISupervisionService _supervisionService;
    private readonly ILogger<ReportService> _logger;

    public ReportService(IReportRepository reportRepo, IOptions<AppServiceOptions> appServiceOptions, ILogger<ReportService> logger, ISupervisionService supervisionService)
    {
        _reportRepo = reportRepo;
        _appServiceOptions = appServiceOptions.Value;
        _logger = logger;
        _supervisionService = supervisionService;
    }

    public async Task<List<FullSupervisor>> GetFullSupervisors(UserRole currentUserRole)
    {
        if (currentUserRole != UserRole.Admin)
        {
            _logger.LogWarning("UnAuthrize Access To Get Full Supervisors by use with role : {0}", currentUserRole.ToString());
            return new List<FullSupervisor>();
        }
        var fullSupervisor = (await _reportRepo.GetFullSupervisors(_appServiceOptions.MaxStudentsPerSupervisor)).Result;
        return fullSupervisor ?? new List<FullSupervisor>();

    }

    public async Task<List<IdelSupervisor>> GetIdleSupervisors(UserRole currentUserRole)
    {
        if (currentUserRole != UserRole.Admin)
        {
            _logger.LogWarning("UnAuthrize Access To Get Idle Supervisors by use with role : {0}", currentUserRole.ToString());
            return new List<IdelSupervisor>();
        }
        var idleSupervisor = (await _reportRepo.GetIdleSupervisors()).Result;
        return idleSupervisor ?? new List<IdelSupervisor>();
    }

    public async Task<OneOf<StudentReport, List<ValidationResult>, List<ServiceMassage>>> GetStudentReport(Guid currentUserId, UserRole currentUserRole, Guid studentId)
    {
        if (currentUserId != studentId && currentUserRole != UserRole.Admin)
        {
            _logger.LogWarning("UnAuthrize Access To Get Student Report by use with id : {0} , role : {1}", currentUserId, currentUserRole.ToString());

            return new List<ServiceMassage> { new ServiceMassage("You Don't have Permission to access this Report ") };
        }
        if (currentUserRole == UserRole.Supervisor)
        {
            var studentSupervisor = await _supervisionService.GetStudentSupervisor(studentId);
            if (studentSupervisor is null || studentSupervisor.id != currentUserId)
            {
                _logger.LogWarning("UnAuthrize Access To Get Student Report by use with id : {0} , role : {1}", currentUserId, currentUserRole.ToString());

                return new List<ServiceMassage> { new ServiceMassage("You Can see Report about your students only") };
            }
        }

        var studentReportResult = (await _reportRepo.GetStudentReport(studentId)).Result;
        if (studentReportResult is null)
        {
            return new List<ServiceMassage> { new ServiceMassage("Can't Get Student Report") };
        }
        return studentReportResult;
    }

    public async Task<OneOf<SupervisorReport, List<ValidationResult>, List<ServiceMassage>>> GetSupervisorReport(Guid currentUserId, UserRole currentUserRole, Guid supervisorId)
    {
        if (currentUserId != supervisorId && currentUserRole != UserRole.Admin)
        {
            _logger.LogWarning("UnAuthrize Access To Get Supervisor Report by use with id : {0} , role : {1}", currentUserId, currentUserRole.ToString());

            return new List<ServiceMassage> { new ServiceMassage("You Don't have Permission to access this Report ") };
        }
        var supervisorReportResult = (await _reportRepo.GetSupervisorReport(supervisorId)).Result;
        if (supervisorReportResult is null)
        {
            return new List<ServiceMassage> { new ServiceMassage("Can't Get Supervisor Report") };
        }
        return supervisorReportResult;
    }

    public async Task<List<TopStudentAbsence>> GetTopStudentsAbsence(UserRole currentUserRole)
    {
        if(currentUserRole != UserRole.Admin)
        {
            _logger.LogWarning("UnAuthrize Access To Get Top Student Absence by use with role : {0}", currentUserRole.ToString());
            return new List<TopStudentAbsence>();
        }
        var topStudentAbsence = (await _reportRepo.GetTopStudentsAbsence(_appServiceOptions.MaxStudentsPerSupervisor)).Result;
        return topStudentAbsence ?? new List<TopStudentAbsence>();
    }

    public async Task<List<TopSupervisorAbsence>> GetTopSupervisorsAbsence(UserRole currentUserRole)
    {
        if (currentUserRole != UserRole.Admin)
        {
            _logger.LogWarning("UnAuthrize Access To Get Top Supervisor Absence by use with role : {0}", currentUserRole.ToString());
            return new List<TopSupervisorAbsence>();
        }
        var topSupervisorAbsence = (await _reportRepo.GetTopSupervisorsAbsence(_appServiceOptions.MaxStudentsPerSupervisor)).Result;
        return topSupervisorAbsence ?? new List<TopSupervisorAbsence>();
    }
}
