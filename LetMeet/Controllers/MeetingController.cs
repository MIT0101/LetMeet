using LetMeet.Business.Interfaces;
using LetMeet.Data.Dtos.Meeting;
using LetMeet.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LetMeet.Business;
using Newtonsoft.Json;
using NuGet.Protocol;
using LetMeet.Models;
using LetMeet.Data.Entites.Meetigs;

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
    //show all meetings for supervisor can have filter for student and date
    [HttpGet]
    public IActionResult Index(Guid id, string filter = null, string value = null, List<string> errors = null, List<string> messages = null)
    {
        ViewData[ViewStringHelper.Errors] = errors ?? new List<string>();
        ViewData[ViewStringHelper.Messages] = messages ?? new List<string>();
        //show all supervisor meetings

        throw new NotImplementedException();
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> Create(Guid id, Guid studentId, List<string> errors, List<string> messages)
    {
        ViewData[ViewStringHelper.Errors] = errors ?? new List<string>();
        ViewData[ViewStringHelper.Messages] = messages ?? new List<string>();

        //get all students for supervisor (List of DatedStudentDto)
        var summaryResult = await _studentsService.GetStudentSummary(studentId);
        ViewData[ViewStringHelper.StudentSummary] = summaryResult.Match(
                       studentSummary => studentSummary
                                  , validationErrors => null
                                             , serviceMessages => null);

        //get all mutual free days for supervisor and student (Dictionary<int, DayHours>)
        var mutalFreeDaysResult = await _supervisionService.GetMutualFreeDay(id, studentId, DateTime.Now);
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
    //feture work 
    //1-create ui to enable user top create new meeting
    //2- create ui to show meetings as cards 
    //, when user click on card he can update meeting info like tasks present when meet is in current time
    //user has show more button that call api to show more based on filter(all,today) 
    //, user has button to redirect him to create new meeting
    //3- create api endpoint to show meetings based on (all || today )
    [HttpPost()]
    public async Task<IActionResult> Create(Guid id )
    {
        List<string> errors = new List<string>();
        List<string> messages = new List<string>();
        // Read request body as a string
        using var reader = new StreamReader(Request.Body);
        string requestBody = await reader.ReadToEndAsync();
        MeetingDto meetingDto = null;
        // Deserialize the JSON content into a MeetingDto object
        try
        {
            meetingDto = JsonConvert.DeserializeObject<MeetingDto>(requestBody);
            var serviceResult = await _meetingService.Create(id, meetingDto);

            Meeting resultMeeting=null; 
            serviceResult.Switch(
                meeting => {resultMeeting=meeting; messages.Add("Meeting Created Successfully"); }
                , validationErrors => errors.AddRange(validationErrors.Select(x => x.ErrorMessage))
                , serviceMessages => errors.AddRange(serviceMessages.Select(x => x.Message)));

            bool isSuccess=resultMeeting is not null;

            return Json(new MeetingApiResponse { isSuccess= isSuccess ,status = isSuccess ? "Created" :"Error", messages = messages, errors = errors, data = resultMeeting }); ; ;
        }
        catch (Exception ex)
        {

            return Json(MeetingApiResponse.Error(new List<string> { "Invalid data"}));
        }




        //return RedirectToAction(nameof(MeetingController.Index), new { errors, messages });
    }

}
