using LetMeet.Data;
using LetMeet.Data.Entites.Meetigs;
using LetMeet.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories.Repository
{
    public class MeetingTaskRepository : IMeetingTaskRepository
    {
        public readonly MainDbContext _mainDb;

        public MeetingTaskRepository(MainDbContext mainDb)
        {
            _mainDb = mainDb;
        }

        public async Task<RepositoryResult<List<MeetingTask>?>> GetTasksByMeetingId(int meetingId)
        {
        
            try
            {
                var foundTasks = await _mainDb.Meetings.Where(x=>x.id == meetingId).Select(x=>x.tasks).SingleOrDefaultAsync();


                if (foundTasks is null)
                {
                    return RepositoryResult<List<MeetingTask>?>.FailureResult(ResultState.NotFound, null, new List<string> { "No Meeting Found " });
                }
                return RepositoryResult<List<MeetingTask>?>.SuccessResult(ResultState.Seccess, foundTasks);

            }
            catch (Exception ex)
            {

                return RepositoryResult<List<MeetingTask>?>.FailureResult(ResultState.DbError, null, new List<string> { "UnExpected Error" });
            }
        }
    }
}
