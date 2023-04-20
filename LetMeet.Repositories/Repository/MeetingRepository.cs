using LetMeet.Data;
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

    public MeetingRepository(MainDbContext mainDb, ISupervisonRepository supervisonRepo)
    {
        _mainDb = mainDb;
        _supervisonRepo = supervisonRepo;
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

            supervision.meetings = supervision.meetings ?? new List<Meeting>();

            supervision.meetings.Add(meeting);

             _mainDb.Update(supervision);

            await _mainDb.SaveChangesAsync();

            return RepositoryResult<Meeting>.SuccessResult(ResultState.Seccess, meeting);
        }
        catch (Exception ex)
        {
          
            return RepositoryResult<Meeting>.FailureResult(ResultState.DbError, null, new List<string> { "UnExpected Error" });
        }
    }

    public async Task<RepositoryResult<Meeting>> GetMeetingOn(Guid supervisorId, Guid studentId, DateTime date)
    {
        try
        {
            var foundMeet = await _mainDb.SupervisionInfo
                .Where(x=>x.supervisor.id== supervisorId&&x.student.id==studentId
                &&x.meetings.Where(m=>m.date.Date==date.Date).FirstOrDefault() != null).Select(x=>x.meetings.FirstOrDefault()).FirstOrDefaultAsync();

            if (foundMeet is null) {
                return RepositoryResult<Meeting>.FailureResult(ResultState.NotFound, null, new List<string> { $"No Meet Found On {date.Date}"});
            }
            return RepositoryResult<Meeting>.SuccessResult(ResultState.Seccess,foundMeet);
        }
        catch (Exception ex)
        {

            return RepositoryResult<Meeting>.FailureResult(ResultState.DbError,null,new List<string> { "UnExpected Error"});
        }
    }
}
