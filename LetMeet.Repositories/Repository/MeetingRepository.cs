using LetMeet.Data;
using LetMeet.Data.Dtos.MeetingsStaff;
using LetMeet.Data.Entites.Meetigs;
using LetMeet.Data.Entites.UsersInfo;
using LetMeet.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

    public async Task<RepositoryResult<List<MeetingFullDto>?>> GetMeetingsAsync(Guid supervisorId, Guid studentId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var foundMeets = await _mainDb.Meetings
                .Where(x => x.SupervisionInfo.supervisor.id == supervisorId && x.SupervisionInfo.student.id == studentId &&
                x.date.Date >= startDate.Date && x.date.Date <= endDate.Date)
                .Select(m=> MeetingFullDto.GetFromMeeting(m)).ToListAsync();
            if (foundMeets is null || foundMeets.Count == 0)
            {
                return RepositoryResult<List<MeetingFullDto>?>.FailureResult(ResultState.NotFound, null, new List<string> { $"No Meet Found Between "+ startDate.Date.ToString("d") + " and "+ endDate.Date.ToString("d") });
            }
            return RepositoryResult<List<MeetingFullDto>?>.SuccessResult(ResultState.Seccess, foundMeets);
        }
        catch (Exception ex)
        {
            return RepositoryResult<List<MeetingFullDto>?>.FailureResult(ResultState.DbError, null, new List<string> { "UnExpected Error" });
        }
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
}
