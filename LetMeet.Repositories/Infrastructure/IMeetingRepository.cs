using LetMeet.Data.Entites.Meetigs;
using LetMeet.Data.Entites.UsersInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories.Infrastructure;

public interface IMeetingRepository
{
    Task<RepositoryResult<Meeting>> AddMeetingToSupervsion(Meeting meeting,SupervisionInfo supervision);
    Task<RepositoryResult<Meeting>> GetMeetingOn(Guid supervisorId, Guid studentId, DateTime date);


}
