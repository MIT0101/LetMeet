using LetMeet.Business.Interfaces;
using LetMeet.Business.Results;
using LetMeet.Data;
using LetMeet.Data.Dtos.Meeting;
using LetMeet.Data.Entites.Meetigs;
using LetMeet.Data.Entites.UsersInfo;
using LetMeet.Repositories;
using LetMeet.Repositories.Infrastructure;
using OneOf;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Business.Implemintation;

public partial class MeetingService : IMeetingService
{
    public readonly IMeetingRepository _meetingRepo;
    private readonly ISupervisonRepository _supervisionRepo;
    private readonly AppTimeProvider _appTimeProvider;

    public MeetingService(IMeetingRepository meetingRepo, ISupervisonRepository supervisionRepo, AppTimeProvider appTimeProvider)
    {
        _meetingRepo = meetingRepo;
        _supervisionRepo = supervisionRepo;
        _appTimeProvider = appTimeProvider;
    }

    public async Task<OneOf<Meeting, IEnumerable<ValidationResult>, IEnumerable<ServiceMassage>>> Create(Guid supervisorId, MeetingDto meetingDto)
    {
        // check if authorize
        if (supervisorId != meetingDto.supervisorId)
        {
            return new List<ValidationResult> { new ValidationResult("You CaN Create Meeting only For Your Students") };

        }
        //validate Data 
        var validationResult = RepositoryValidationResult.DataAnnotationsValidation(meetingDto);
        if (!validationResult.IsValid)
        {
            return validationResult.ValidationErrors;

        }

        // check is there a supervision between supervisor and student if not return validation error 
        var supervison = (await _supervisionRepo.GetSupervision_Meetings_FreeDaysAsync(supervisorId: meetingDto.supervisorId, studentId: meetingDto.studentId)).Result;
        if (supervison == null)
        {
            return new List<ValidationResult> { new ValidationResult($"No Supervision Between These Users") };
        }
        if (supervison.endDate <= _appTimeProvider.Now)
        {
            return new List<ServiceMassage> { new ServiceMassage($"Supervision has Expired Since {supervison.endDate.Date}") };

        }
        //get supervisor and student mutual free days use DayHour
        Dictionary<int, DayHours> mutualDays = GetMutualDays(supervison.supervisor.freeDays, supervison.student.freeDays);

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
        if (!mutualDays[(int)meetingDto.date.DayOfWeek].CanAdd(meetingDto.startHour, meetingDto.endHour))
        {
            return new List<ServiceMassage> { new ServiceMassage($"There is No Available Free Hours is {meetingDto.date.DayOfWeek}") };
        }
        //check there is any meet at same date if there check if can add meet at same day
        var meetAtSameDate = (await _meetingRepo.GetMeetingOn(meetingDto.supervisorId, meetingDto.studentId, meetingDto.date)).Result;
        if (meetAtSameDate is not null
            && new DayHours((int)meetAtSameDate.date.DayOfWeek, meetAtSameDate.startHour, meetAtSameDate.endHour)
            .CanAdd(meetingDto.startHour, meetingDto.endHour))
        {
            return new List<ServiceMassage> { new ServiceMassage($"There is An Existing Meet At Same Date {meetAtSameDate.date.Date}") };

        }
        //create meeting and save it 
        Meeting meeting = new Meeting
        {
            date = meetingDto.date,
            startHour = meetingDto.startHour,
            endHour = meetingDto.endHour,
            totalTimeHoure = meetingDto.totalTimeHoure,
            description = meetingDto.description,
            isPresent = meetingDto.isPresent,
            tasks = meetingDto.tasks?.Select(t => new MeetingTask { title = t.title, decription = t.decription, isCompleted = t.isCompleted }).ToList()

        };
        //must add meet to supervision meetings 

        var reposResult = await _meetingRepo.AddMeetingToSupervsion(meeting, supervison);

        if (!reposResult.Success)
        {
            return new List<ServiceMassage> { new ServiceMassage("Can Not Create Meeting") };
        }
        return meeting;
    }

    public OneOf<Dictionary<int, DayHours>> GetMutalDays(Guid supervisorId, Guid studentId, DateTime date)
    {
        throw new NotImplementedException();

    }

    private Dictionary<int, DayHours> GetMutualDays(List<DayFree> list1, List<DayFree> list2)
    {
        list1 = list1 ?? new List<DayFree>();
        list2 = list2 ?? new List<DayFree>();
        Dictionary<int, DayFree> firstMap = new Dictionary<int, DayFree>();
        Dictionary<int, DayHours> mutualDays = new Dictionary<int, DayHours>();
        for (int i = 0; i < list1.Count; i++)
        {
            firstMap.Add(list1[i].day, list1[i]);
        }

        for (int i = 0; i < list2.Count; i++)
        {
            if (firstMap.ContainsKey(list2[i].day))
            {
                DayFree map1Day = firstMap[list1[i].day];
                DayFree map2Day = list2[i];
                DayHours day = new DayHours(map1Day.day, map1Day.startHour, map1Day.endHour);
                day.AddFreeHours(map2Day.startHour, map2Day.endHour);
                mutualDays.Add(day.day, day);
            }
        }

        return mutualDays;

    }
}
