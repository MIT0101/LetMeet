using LetMeet.Business.Interfaces;
using LetMeet.Data.Dtos.Reports;
using LetMeet.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;

namespace LetMeet.Web.Controllers;

public class ReportController : Controller
{
    private readonly IProfileService _profileService;
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService, IProfileService profileService)
    {
        _reportService = reportService;
        _profileService = profileService;
    }

    [HttpGet("/[Controller]/Admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var currentUserId = GenricControllerHelper.GetUserInfoId(User);
        var currentUserRole = GenricControllerHelper.GetUserRole(User);
        ViewData[ViewStringHelper.AllSupervisors] = await _profileService.GetAllSupervisors(currentUserId, currentUserRole);
        ViewData[ViewStringHelper.AllStudents] = await _profileService.GetAllStudents(currentUserId, currentUserRole);
        //get top student absence
        ViewData[ViewStringHelper.TopStudentAbsence] = await _reportService.GetTopStudentsAbsence(currentUserRole);
        //get top supervisor absence
        ViewData[ViewStringHelper.TopSupervisorAbsence] = await _reportService.GetTopSupervisorsAbsence(currentUserRole);
        //get full supervisors
        ViewData[ViewStringHelper.FullSupervisors] = await _reportService.GetFullSupervisors(currentUserRole);
        //get idle supervisors
        ViewData[ViewStringHelper.IdelSupervisors] = await _reportService.GetIdleSupervisors(currentUserRole);

        return View();
    }

    [HttpGet("/[Controller]/Student/{id}")]
    [Authorize]

    public async Task<IActionResult> Student(Guid id)
    {
        var studentReportResult = await _reportService.GetStudentReport(GenricControllerHelper.GetUserInfoId(User), GenricControllerHelper.GetUserRole(User), id);
        StudentReport studentReport = null;
        List<string> errors = new List<string>();
        List<string> messages = new List<string>();
        ViewData[ViewStringHelper.Errors]=errors;
        ViewData[ViewStringHelper.Messages]= messages;

        studentReportResult.Switch(
                       studentReportResultData => studentReport = studentReportResultData,
                       validationErrors => errors.AddRange(validationErrors.Select(v=>v.ErrorMessage)),
                       serviceMessages => errors.AddRange(serviceMessages.Select(sm=>sm.Message))
                        );

        ViewData[ViewStringHelper.StudentReport] = studentReport;

        return View();
    }

    [HttpGet("/[Controller]/Supervisor/{id}")]
    [Authorize(Roles = "Admin , Supervisor")]
    public async Task<IActionResult> Supervisor(Guid id)
    {
        var supervisorReportResult = await _reportService.GetSupervisorReport(GenricControllerHelper.GetUserInfoId(User), GenricControllerHelper.GetUserRole(User), id);
        SupervisorReport supervisorReport = null;
        List<string> errors = new List<string>();
        List<string> messages = new List<string>();
        ViewData[ViewStringHelper.Errors] = errors;
        ViewData[ViewStringHelper.Messages] = messages;

        supervisorReportResult.Switch(
                       supervisorReportResultData => supervisorReport = supervisorReportResultData,
                       validationErrors => errors.AddRange(validationErrors.Select(v => v.ErrorMessage)),
                       serviceMessages => errors.AddRange(serviceMessages.Select(sm => sm.Message))
                        );

        ViewData[ViewStringHelper.SupervisorReport] = supervisorReport;

        return View();
    }
}
