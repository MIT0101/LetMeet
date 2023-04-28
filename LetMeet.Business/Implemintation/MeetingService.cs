using LetMeet.Business.Interfaces;
using LetMeet.Business.Results;
using LetMeet.Data;
using LetMeet.Data.Dtos.MeetingsStaff;
using LetMeet.Data.Entites.Identity;
using LetMeet.Data.Entites.Meetigs;
using LetMeet.Data.Entites.UsersInfo;
using LetMeet.Repositories;
using LetMeet.Repositories.Infrastructure;
using Microsoft.Extensions.Options;
using OneOf;
using System.ComponentModel.DataAnnotations;

namespace LetMeet.Business.Implemintation;

public partial class MeetingService : IMeetingService
{
    public readonly IMeetingRepository _meetingRepo;
    private readonly ISupervisonRepository _supervisionRepo;
    private readonly IUserProfileRepository _userProfileRepo;
    private readonly AppTimeProvider _appTimeProvider;
    private readonly AppServiceOptions _serviceOptions;

    public MeetingService(IMeetingRepository meetingRepo, ISupervisonRepository supervisionRepo, AppTimeProvider appTimeProvider, IUserProfileRepository userProfileRepo, IOptions<AppServiceOptions> appServiceOptions)
    {
        _meetingRepo = meetingRepo;
        _supervisionRepo = supervisionRepo;
        _appTimeProvider = appTimeProvider;
        _userProfileRepo = userProfileRepo;
        _serviceOptions = appServiceOptions.Value;
    }

    public async Task<OneOf<Meeting, IEnumerable<ValidationResult>, IEnumerable<ServiceMassage>>> Create(Guid supervisorId, MeetingDto meetingDto)
    {
        // check if the create is the same supervisor that is logged in
        if (supervisorId != meetingDto.supervisorId)
        {
            return new List<ValidationResult> { new ValidationResult("You CaN Create Meeting only For Your Students") };
        }
        //validate Data MeetingDto data
        var validationErrors = ValidateMeetingDtoData(meetingDto);
        if (validationErrors is not null && validationErrors.Any())
        {
            return validationErrors;
        }
        var supervison = (await _supervisionRepo.GetSupervisionAsync(supervisorId: supervisorId, studentId: meetingDto.studentId)).Result;
        // check is there a supervision between supervisor and student if not return validation error 
        if (supervison == null)
        {
            return new List<ValidationResult> { new ValidationResult($"No Supervision Between These Users") };
        }
        //is supervision expired if yes return message
        if (supervison.endDate <= _appTimeProvider.Now)
        {
            return new List<ServiceMassage> { new ServiceMassage($"Supervision has Expired Since {supervison.endDate.Date}") };
        }
        //start new code
        var supervisorFreeDaysList = (await _userProfileRepo.GetFreeDaysAsync(supervisorId)).Result;
        var studentFreeDaysList = (await _userProfileRepo.GetFreeDaysAsync(meetingDto.studentId)).Result;

        //get supervisor and student mutual free days use DayHour
        Dictionary<int, DayHours> mutualDays = DayHours.GetMutualDays(supervisorFreeDaysList, studentFreeDaysList);

        // if there is no mutual free days return Message 
        if (mutualDays.Count < 1)
        {
            return new List<ServiceMassage> { new ServiceMassage("No Mutual Days Found") };
        }
        // if requested day is not in mutual days 
        if (!mutualDays.ContainsKey((int)meetingDto.date.DayOfWeek))
        {
            return new List<ServiceMassage> { new ServiceMassage($"{meetingDto.date.DayOfWeek} Is Not In Free Times") };
        }

        //check if we can add this meeting to this day using DayHours if not return message say cant add to this day
        if (!mutualDays[(int)meetingDto.date.DayOfWeek].CanAddMeet(meetingDto.startHour, meetingDto.endHour))
        {
            return new List<ServiceMassage> { new ServiceMassage($"There is No Available Free Hours on {meetingDto.date.DayOfWeek} , Try another Date") };
        }
        //check there is any meet at same date if there check if can add meet at same day
        var meetsAtSameDate = (await _meetingRepo.GetMeetingsOn(meetingDto.supervisorId, meetingDto.studentId, meetingDto.date)).Result;
        if (meetsAtSameDate is not null && meetsAtSameDate.Count > 0)
        {

            foreach (var meet in meetsAtSameDate)
            {
                //check if there any meet at same date and time
                if (!new DayHours((int)meet.date.DayOfWeek, meet.startHour, meet.endHour).IsSaveToShareHours(meetingDto.startHour, meetingDto.endHour))
                {
                    return new List<ServiceMassage> { new ServiceMassage("There is an Existing Meetings on " + meetingDto.date.ToString("D") + ", Try another Date") };
                }
            }
        }
        //create meeting and save it 
        Meeting meeting = new Meeting
        {
            date = meetingDto.date.AddHours(meetingDto.startHour),
            startHour = meetingDto.startHour,
            endHour = meetingDto.endHour,
            totalTimeHoure = meetingDto.totalTimeHoure,
            description = meetingDto.description,
            isStudentPresent = false,
            isSupervisorPresent = false,
            tasks = meetingDto.hasTasks ? meetingDto.tasks?.Select(t => new MeetingTask { title = t.title, decription = t.description, }).ToList() : null,

        };
        //must add meet to supervision meetings 

        var reposResult = await _meetingRepo.AddMeetingToSupervsion(meeting, supervison);

        if (!reposResult.Success)
        {
            return new List<ServiceMassage> { new ServiceMassage("Can Not Create Meeting") };
        }
        return meeting;
    }

    public async Task<OneOf<List<MeetingFullDto>, IEnumerable<ValidationResult>, IEnumerable<ServiceMassage>>> GetMeetings(Guid currentUserId, UserRole userRole, MeetingQuery query)
    {
        //check if query is null
        if (query is null)
        {
            return new List<ServiceMassage> { new ServiceMassage("Your Meeting Query Can Not Be Empty") };
        }
        //validate query using data annotation 
        var validationErrors = ValidateMeetingQueryData(query);

        if (validationErrors is not null && validationErrors.Any())
        {
            return validationErrors;
        }
        //check if the current user is not a supervisor or student or not admin
        if ((currentUserId != query.studentId && currentUserId != query.supervisorId) && userRole != UserRole.Admin)
        {
            return new List<ServiceMassage> { new ServiceMassage("You Can Not Get Meetings For Other Users") };
        }
        var repoResult = await _meetingRepo.GetMeetingsBetween(query);
        List<MeetingFullDto>? meetings = repoResult.Result;
        if (repoResult.State == ResultState.DbError)
        {
            return new List<ServiceMassage> { new ServiceMassage("Can Not Get Meetings") };
        }
        if (meetings is null || meetings?.Count < 1)
        {
            return new List<ServiceMassage> { new ServiceMassage("No Meetings Found") };
        }
        return meetings;
    }

    // validate meeting query data
    private List<ValidationResult>? ValidateMeetingQueryData(MeetingQuery query)
    {
        var validationResult = RepositoryValidationResult.DataAnnotationsValidation(query);
        if (!validationResult.IsValid)
        {
            return validationResult.ValidationErrors;
        }
        // if tart hour is greater or equal than end hour return validation error
        if (query.startDate.Date >= query.endDate.Date)
        {
            return new List<ValidationResult> { new ValidationResult("Start Date Must Be Less Than End Hour", new string[] { nameof(query.startDate), nameof(query.endDate) }) };
        }
        return null;

    }

    //validate meetingDto
    private List<ValidationResult>? ValidateMeetingDtoData(MeetingDto meetingDto)
    {
        var validationResult = RepositoryValidationResult.DataAnnotationsValidation(meetingDto);
        if (!validationResult.IsValid)
        {
            return validationResult.ValidationErrors;
        }
        // if tart hour is greater or equal than end hour return validation error
        if (meetingDto.startHour >= meetingDto.endHour)
        {
            return new List<ValidationResult> { new ValidationResult("Start Hour Must Be Less Than End Hour", new string[] { nameof(meetingDto.startHour), nameof(meetingDto.endHour) }) };
        }

        // check if the day of meetingDto date is not equal meetingDto day
        if ((int)meetingDto.date.Date.DayOfWeek != meetingDto.day)
        {
            return new List<ValidationResult> { new ValidationResult($"You Must Select Date with {(DayOfWeek)meetingDto.day} Day", new string[] { nameof(meetingDto.day) }) };
        }

        return null;
    }

    public async Task<OneOf<Meeting, IEnumerable<ValidationResult>, IEnumerable<ServiceMassage>>> RemoveMeeting(Guid currentUserId, UserRole userRole, int meetingId)
    {

        //get meeting
        var meeting = (await _meetingRepo.GetMeetingAsync(meetingId)).Result;
        if (meeting is null)
        {
            return new List<ServiceMassage> { new ServiceMassage("Meeting Not Found") };
        }
        //check if the current user is not a supervisor or student or not admin
        if ((currentUserId != meeting.SupervisionInfo.supervisor.id) && userRole != UserRole.Admin)
        {
            return new List<ServiceMassage> { new ServiceMassage("You Can Not Remove Meetings For Other Users") };
        }
        // check if we can delete this meeting using MeetingFullDto
        MeetingFullDto meetingFullDto = new MeetingFullDto
        {
            startHour = meeting.startHour,
            endHour = meeting.endHour,
            date = meeting.date,

        };

        if (!meetingFullDto.CanDelete(_appTimeProvider.Now, _serviceOptions.PaddingMeetHours)) {
            return new List<ServiceMassage> { new ServiceMassage("You Can Not Remove Meeting") };
        }

        //remove meeting
        var repoResult = await _meetingRepo.RemoveMeetingAsync(meetingId);
        if (repoResult.State == ResultState.DbError || repoResult.State != ResultState.Seccess)
        {
            return new List<ServiceMassage> { new ServiceMassage("Can Not Remove Meeting") };
        }
        return meeting;

    }
}
