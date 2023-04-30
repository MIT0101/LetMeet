using LetMeet.Data.Entites.Meetigs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories.Infrastructure
{
    public interface IMeetingTaskRepository
    {
        Task<RepositoryResult<List<MeetingTask>?>> GetTasksByMeetingId(int meetingId);
    }
}
