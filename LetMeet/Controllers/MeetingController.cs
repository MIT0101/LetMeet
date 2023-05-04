using LetMeet.Business.Interfaces;
using LetMeet.Data.Dtos.MeetingsStaff;
using LetMeet.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using LetMeet.Models;
using LetMeet.Data.Entites.Meetigs;
using LetMeet.Data.Dtos.Supervision;
using LetMeet.Web.Models;

namespace LetMeet.Controllers;

[Authorize]
public class MeetingController : Controller
{

    private readonly IMeetingService _meetingService;
    private readonly ISupervisionService _supervisionService;
    private readonly IStudentsService _studentsService;

    private readonly IHttpContextAccessor _contextAccessor;
    public MeetingController(IMeetingService meetingService, ISupervisionService supervisionService, IStudentsService studentService, IHttpContextAccessor contextAccessor)
    {
        _meetingService = meetingService;
        _supervisionService = supervisionService;
        _studentsService = studentService;
        _contextAccessor = contextAccessor;
    }

    //Register Student Presence and supervisor obscene
    [HttpPost("/Meetings/api/RegisterStudentPresence/{id}")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<IAppApiResponse>> RegisterStudentPresence(int id)
    {
        List<string> errors = new List<string>();
        List<string> messages = new List<string>();
        Meeting? meeting = null;
        var result = await _meetingService.RegisterStudentPresence(GenricControllerHelper.GetUserInfoId(User), GenricControllerHelper.GetUserRole(User), id);
        result.Switch(
                      meetingDto => { meeting = meetingDto; messages.Add("Student presence registered successfully"); }
                      , validationErrors => errors.AddRange(validationErrors.Select(x => x.ErrorMessage))
                      , serviceMessages => errors.AddRange(serviceMessages.Select(x => x.Message)));
        bool isSuccess = meeting is not null;
        return Json(new MeetingApiResponse { isSuccess = isSuccess, status = isSuccess ? "Registered" : "Error", messages = messages, errors = errors, data = meeting }); ; ;
    }

    //Complete Meeting
    [HttpPost("/[Controller]/api/CompleteMeeting")]
    [Authorize(Roles = "Supervisor")]
    public async Task<ActionResult<IAppApiResponse>> CompleteMeeting([FromBody] CompleteMeetingDto meetingDto)
    {

        List<string> errors = new List<string>();
        List<string> messages = new List<string>();
        Meeting? completedMeeting = null;

        var result = await _meetingService.CompleteMeeting(GenricControllerHelper.GetUserInfoId(User), GenricControllerHelper.GetUserRole(User), meetingDto);
        result.Switch(
                       meeting => { completedMeeting = meeting; messages.Add("Meeting completed successfully"); }
                       , validationErrors => errors.AddRange(validationErrors.Select(x => x.ErrorMessage))
                        , serviceMessages => errors.AddRange(serviceMessages.Select(x => x.Message)));
        bool isSuccess = completedMeeting is not null;

        return Json(new MeetingApiResponse { isSuccess = isSuccess, status = isSuccess ? "Completed" : "Error", messages = messages, errors = errors, data = completedMeeting }); ; ;

    }


    // remove meeting 
    [HttpPost("/[Controller]/api/Remove/{id}")]
    public async Task<ActionResult<IAppApiResponse>> Remove(int id)
    {
        List<string> errors = new List<string>();
        List<string> messages = new List<string>();

        // remove meeting
        var result = await _meetingService.RemoveMeeting(GenricControllerHelper.GetUserInfoId(User), GenricControllerHelper.GetUserRole(User), id);
        MeetingDeleteRecoDto? resultMeeting = null;
        result.Switch(
                       meetingDto => { resultMeeting = meetingDto; messages.Add("Meeting removed successfully"); }
                      , validationErrors => errors.AddRange(validationErrors.Select(x => x.ErrorMessage))
                       , serviceMessages => errors.AddRange(serviceMessages.Select(x => x.Message)));
        bool isSuccess = resultMeeting is not null;
        return Json(new MeetingDeleteApiResponse { isSuccess = isSuccess, status = isSuccess ? "Removed" : "Error", messages = messages, errors = errors, data = resultMeeting }); ; ;
    }

    // show meetings for admin
    [HttpGet("/[Controller]/Admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminSearch(List<string> errors = null, List<string> messages = null)
    {
        var supervsions = await _supervisionService.GetSupervisionsSummary(GenricControllerHelper.GetUserRole(User));
        ViewData[ViewStringHelper.SupervsionSummary] = supervsions;


        MeetingQuery? query = await GetMeetingQueryForAdmin(_contextAccessor.HttpContext.Request, supervsions);
        return await Search(query, GenricControllerHelper.GetUserInfoId(User), GenricControllerHelper.GetUserRole(User), errors, messages);
    }
    // show meetings for student
    [HttpGet("/[Controller]/Student")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Student(List<string> errors = null, List<string> messages = null)
    {
        MeetingQuery? query = await GetMeetingQueryForStudent(_contextAccessor.HttpContext.Request, GenricControllerHelper.GetUserInfoId(User), GenricControllerHelper.GetUserRole(User));
        return await Search(query, GenricControllerHelper.GetUserInfoId(User), GenricControllerHelper.GetUserRole(User), errors, messages);
    }
    // show meetings for supervisor
    [HttpGet("/[Controller]/Supervisor")]
    [Authorize(Roles = "Supervisor")]
    public async Task<IActionResult> Supervisor(List<string> errors = null, List<string> messages = null)
    {
        MeetingQuery? query = await GetMeetingQueryForSupervisor(_contextAccessor.HttpContext.Request, GenricControllerHelper.GetUserInfoId(User), GenricControllerHelper.GetUserRole(User));
        ViewData[ViewStringHelper.SupervisorStudents] = await _supervisionService.GetAllSupervisorStudents(GenricControllerHelper.GetUserInfoId(User));
        return await Search(query, GenricControllerHelper.GetUserInfoId(User), GenricControllerHelper.GetUserRole(User), errors, messages);
    }
    private async Task<IActionResult> Search(MeetingQuery? query, Guid currentUserInfoId, UserRole currentUserRole, List<string> errors = null, List<string> messages = null)
    {
        InitAndAssginErrorsAndMessagesForView(ref errors, ref messages);

        List<MeetingFullDto> meetings = new List<MeetingFullDto>();
        ViewData[ViewStringHelper.Meetings] = meetings;
        ViewData[ViewStringHelper.RequestedMeetingQuery] = query;
        string supervisorName = string.Empty;
        string studentName = string.Empty;
        if (query is null)
        {
            return BadRequest();
        }

        //get supervisor name for supervisorId in query
        if (query.supervisorId != null)
        {
            SupervisorOrStudentSelectDto? supervisorResult = await _supervisionService.GetSupervisorOrStudent(query.supervisorId);
            supervisorName = supervisorResult?.FullName;
        }
        //get student name for studentId in query
        if (query.studentId != null)
        {
            SupervisorOrStudentSelectDto? studentResult = await _supervisionService.GetSupervisorOrStudent(query.studentId);
            studentName = studentResult?.FullName;
        }

        ViewData[ViewStringHelper.RequestedSupervisorName] = supervisorName;
        ViewData[ViewStringHelper.RequestedStudentName] = studentName;
        //Get All Meeting for current data use MeetingQuery and Meeting service use switch to handle result
        var meetingsResult = await _meetingService.GetMeetings(currentUserInfoId, currentUserRole, query);
        meetingsResult.Switch(
                       meetingsDto => meetings.AddRange(meetingsDto)
                       , validationErrors => errors.AddRange(validationErrors.Select(x => x.ErrorMessage))
                       , serviceMessages => errors.AddRange(serviceMessages.Select(x => x.Message)));

        return View("ShowMeetings");
    }

    //show create meeting form
    [HttpGet]
    [Authorize(Roles = "Supervisor")]
    public async Task<IActionResult> Create(Guid studentId, List<string> errors, List<string> messages)
    {
        InitAndAssginErrorsAndMessagesForView(ref errors, ref messages);

        //get all students for supervisor (List of DatedStudentDto)
        var summaryResult = await _studentsService.GetStudentSummary(studentId);
        ViewData[ViewStringHelper.StudentSummary] = summaryResult.Match(
                       studentSummary => studentSummary
                                  , validationErrors => null
                                             , serviceMessages => null);

        //get all mutual free days for supervisor and student (Dictionary<int, DayHours>)
        var mutalFreeDaysResult = await _supervisionService.GetMutualFreeDay(GenricControllerHelper.GetUserInfoId(User), studentId, DateTime.Now);
        Dictionary<int, ISet<int>> daysFreeHours = new Dictionary<int, ISet<int>>();

        mutalFreeDaysResult.Switch(
                        mutualFreeDays =>
                        {
                            foreach (var freeDay in mutualFreeDays.Values)
                            {
                                daysFreeHours.Add(freeDay.day, freeDay.GetFreeHours());
                            }
                        }
                         , validationErrors => { }
                         , serviceMessages => { });
        ViewData[ViewStringHelper.MutualFreeDays] = daysFreeHours;


        return View();
    }
    [HttpPost("/[Controller]/api/Create")]
    [Authorize(Roles = "Supervisor")]
    public async Task<ActionResult<IAppApiResponse>> Create()
    {
        List<string> errors = new List<string>();
        List<string> messages = new List<string>();
        // Read request body as a string
        using var reader = new StreamReader(Request.Body);
        string requestBody = await reader.ReadToEndAsync();
        MeetingDto meetingDto = null;
        // De-serialize the JSON content into a MeetingDto object
        try
        {
            meetingDto = JsonConvert.DeserializeObject<MeetingDto>(requestBody);
            var serviceResult = await _meetingService.Create(GenricControllerHelper.GetUserInfoId(User), meetingDto);

            Meeting resultMeeting = null;
            serviceResult.Switch(
                meeting => { resultMeeting = meeting; messages.Add("Meeting Created Successfully"); }
                , validationErrors => errors.AddRange(validationErrors.Select(x => x.ErrorMessage))
                , serviceMessages => errors.AddRange(serviceMessages.Select(x => x.Message)));

            bool isSuccess = resultMeeting is not null;

            return Json(new MeetingApiResponse { isSuccess = isSuccess, status = isSuccess ? "Created" : "Error", messages = messages, errors = errors, data = resultMeeting }); ; ;
        }
        catch (Exception ex)
        {

            return Json(MeetingApiResponse.Error(new List<string> { "Invalid data" }));
        }

        //return RedirectToAction(nameof(MeetingController.Index), new { errors, messages });
    }
    /**********************************************------------GET MEETING QUERY --------------****************************************/
    //for students
    private async Task<MeetingQuery?> GetMeetingQueryForStudent(HttpRequest request, Guid currentUserId, UserRole currentUserRole)
    {
        MeetingQuery query = new MeetingQuery();
        //get object of MeetingQuery from query string
        try
        {
            InitalStartEndDateMeetingQuery(request, ref query);
            query.studentId = currentUserId;
            var supervisor = await _supervisionService.GetStudentSupervisor(currentUserId);
            if (supervisor is not null) { 
            query.supervisorId = (await _supervisionService.GetStudentSupervisor(currentUserId)).id;
            }
            return query;
        }
        catch (Exception ex)
        {
            return null;
        }

    }
    //for supervisors
    private async Task<MeetingQuery?> GetMeetingQueryForSupervisor(HttpRequest request, Guid currentUserId, UserRole currentUserRole)
    {
        MeetingQuery query = new MeetingQuery();
        //get object of MeetingQuery from query string
        try
        {
            query.supervisorId = currentUserId;
            InitalStartEndDateMeetingQuery(request, ref query);
            try
            {
                query.studentId = Guid.Parse(Request.Query[QueryStringHelper.MeetingQueryStudentId]);
            }
            catch (Exception ex)
            {
                //if current user is supervisor and the query supervisor id equal to currentUserId and query.studentId is empty make query.studentId is the first student of supervisor
                if (currentUserRole == UserRole.Supervisor && currentUserId == query.supervisorId)
                {
                    // make query.studentId is the first student of supervisor 
                    var allStudents = await _supervisionService.GetAllSupervisorStudents(currentUserId);
                    if (allStudents.Any())
                    {
                        query.studentId = allStudents.FirstOrDefault().id;
                    }
                }
            }

            return query;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    //for admin
    private async Task<MeetingQuery>? GetMeetingQueryForAdmin(HttpRequest request, List<SupervsionSummary>? supervsions)
    {
        MeetingQuery query = new MeetingQuery();
        //get object of MeetingQuery from query string
        try
        {
            InitalStartEndDateMeetingQuery(request, ref query);

            try
            {
                //get supervsionId from query string
                string supervsionIdStr = request.Query[QueryStringHelper.MeetingQuerySupervsionId];
                int supervsionId= int.Parse(supervsionIdStr);

                var supervsion = supervsions?.FirstOrDefault(x => x.supervsionId == supervsionId);

                if (supervsion != null)
                {
                    query.supervisorId = supervsion.supervisorId;
                    query.studentId = supervsion.studentId;
                }

                return query;

        }
        catch (Exception ex)
        {
            var supervision = supervsions?.FirstOrDefault();

            if (supervision is null)
            {
                return null;
            }
            query.supervisorId = supervision.supervisorId;
            query.studentId = supervision.studentId;

        }
        return query;

    }
        catch (Exception ex)
        {
            return null;
        }
    }
    private void InitalStartEndDateMeetingQuery(HttpRequest request, ref MeetingQuery query)
{
    query.startDate = DateTime.Parse(request.Query[QueryStringHelper.MeetingQueryStartDate]);
    query.endDate = DateTime.Parse(request.Query[QueryStringHelper.MeetingQueryEndDate]);
}
private void InitAndAssginErrorsAndMessagesForView(ref List<string>? errors, ref List<string>? messages)
{
    errors ??= new List<string>();
    messages ??= new List<string>();
    ViewData[ViewStringHelper.Errors] = errors;
    ViewData[ViewStringHelper.Messages] = messages;
}
}
