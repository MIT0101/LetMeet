using LetMeet.Data;
using LetMeet.Data.Dtos.MeetingsStaff;
using LetMeet.Data.Entites.Meetigs;
using LetMeet.Data.Entites.UsersInfo;
using LetMeet.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories.Repository;

public class MeetingRepository : IMeetingRepository
{
    private readonly MainDbContext _mainDb;
    private readonly ISupervisonRepository _supervisonRepo;
    private readonly AppTimeProvider _appTimeProvider;

    public MeetingRepository(MainDbContext mainDb, ISupervisonRepository supervisonRepo, AppTimeProvider appTimeProvider)
    {
        _mainDb = mainDb;
        _supervisonRepo = supervisonRepo;
        _appTimeProvider = appTimeProvider;
    }

    public async Task<RepositoryResult<Meeting>> AddMeetingToSupervsion(Meeting meeting, SupervisionInfo supervision)
    {

        try
        {
            var validationErrors = RepositoryValidationResult.DataAnnotationsValidation(meeting);
            if (!validationErrors.IsValid)
            {
                return RepositoryResult<Meeting>.FailureValidationResult(validationErrors.ValidationErrors);
            }
            meeting.created = _appTimeProvider.Now;
            meeting.SupervisionInfo = supervision;

            await _mainDb.Meetings.AddAsync(meeting);

            await _mainDb.SaveChangesAsync();

            return RepositoryResult<Meeting>.SuccessResult(ResultState.Seccess, meeting);
        }
        catch (Exception ex)
        {
          
            return RepositoryResult<Meeting>.FailureResult(ResultState.DbError, null, new List<string> { "UnExpected Error" });
        }
    }

    public async Task<RepositoryResult<Meeting?>> GetMeetingAsync(int meetingId)
    {
        try
        {
            var foundMeet = await _mainDb.Meetings.Include(x => x.SupervisionInfo.supervisor).Include(x=>x.SupervisionInfo.student)
                .Where(x=>x.id==meetingId)
                .FirstOrDefaultAsync();
                
            if (foundMeet is null)
            {
                return RepositoryResult<Meeting?>.FailureResult(ResultState.NotFound, null, new List<string> { "No Meeting Found " });
            }
            return RepositoryResult<Meeting?>.SuccessResult(ResultState.Seccess, foundMeet);

        }
        catch (Exception ex)
        {

            return RepositoryResult<Meeting?>.FailureResult(ResultState.DbError, null, new List<string> { "UnExpected Error" });
        }
    }

    public async Task<RepositoryResult<List<MeetingFullDto>?>> GetMeetingsAsync(Expression<Func<Meeting, bool>> filter)
    {
        try
        {
            var foundMeets = await _mainDb.Meetings.Include(x => x.tasks)
                .Where(filter)
                .Select(m=> MeetingFullDto.GetFromMeeting(m,m.SupervisionInfo.supervisor.id,m.SupervisionInfo.student.id,m.SupervisionInfo.supervisor.fullName,m.SupervisionInfo.student.fullName)).ToListAsync();
            if (foundMeets is null || foundMeets.Count == 0)
            {
                return RepositoryResult<List<MeetingFullDto>?>.FailureResult(ResultState.NotFound, null, new List<string> { "No Meeting Found " });
            }
            return RepositoryResult<List<MeetingFullDto>?>.SuccessResult(ResultState.Seccess, foundMeets);
        }
        catch (Exception ex)
        {
            return RepositoryResult<List<MeetingFullDto>?>.FailureResult(ResultState.DbError, null, new List<string> { "UnExpected Error" });
        }
    }
    public async Task<RepositoryResult<List<MeetingFullDto>?>> GetMeetingsBetween(MeetingQuery query)
    { 
    return await GetMeetingsAsync(x => x.SupervisionInfo.supervisor.id == query.supervisorId
        && x.SupervisionInfo.student.id == query.studentId && x.date.Date >= query.startDate.Date && x.date.Date <= query.endDate.Date);
    }
    public async Task<RepositoryResult<List<Meeting>?>> GetMeetingsOn(Guid supervisorId, Guid studentId, DateTime date)
    {
        try
        {
            var foundMeets = await _mainDb.Meetings
                .Where(x => x.SupervisionInfo.supervisor.id == supervisorId && x.SupervisionInfo.student.id == studentId && x.date.Date == date.Date).ToListAsync();

            if (foundMeets is null || foundMeets.Count ==0) {
                return RepositoryResult<List<Meeting>?>.FailureResult(ResultState.NotFound, null, new List<string> { $"No Meet Found On {date.Date}"});
            }
            return RepositoryResult<List<Meeting>?>.SuccessResult(ResultState.Seccess,foundMeets);
        }
        catch (Exception ex)
        {

            return RepositoryResult<List<Meeting>?>.FailureResult(ResultState.DbError,null,new List<string> { "UnExpected Error"});
        }
    }

    public async Task<RepositoryResult<Meeting?>> RemoveMeetingAsync(int meetingId)
    {
        try
        {
            var foundMeeting = await _mainDb.Meetings.Include(x=>x.tasks).Where(x => x.id == meetingId).FirstOrDefaultAsync();

            if (foundMeeting is null)
            {
                return RepositoryResult<Meeting?>.FailureResult(ResultState.NotFound, null, new List<string> { "No Meeting Found " });
            }
            _mainDb.Meetings.Remove(foundMeeting);
            _mainDb.MeetingTasks.RemoveRange(foundMeeting.tasks);

            await _mainDb.SaveChangesAsync();
            return RepositoryResult<Meeting?>.SuccessResult(ResultState.Seccess, foundMeeting);
        }
        catch (Exception ex)
        {
            return RepositoryResult<Meeting?>.FailureResult(ResultState.DbError, null, new List<string> { "UnExpected Error" });
        }
    }
}
