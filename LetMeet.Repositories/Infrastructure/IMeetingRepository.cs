using LetMeet.Data.Dtos.MeetingsStaff;
using LetMeet.Data.Entites.Meetigs;
using LetMeet.Data.Entites.UsersInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Repositories.Infrastructure;

public interface IMeetingRepository
{
    Task<RepositoryResult<Meeting>> AddMeetingToSupervsion(Meeting meeting,SupervisionInfo supervision);
    Task<RepositoryResult<List<MeetingFullDto>?>> GetMeetingsBetween(MeetingQuery query);
    Task<RepositoryResult<List<MeetingFullDto>?>> GetMeetingsAsync(Expression<Func<Meeting, bool>> filter);
    Task<RepositoryResult<List<Meeting>?>> GetMeetingsOn(Guid supervisorId, Guid studentId, DateTime date);
    Task<RepositoryResult<Meeting?>> GetMeetingAsync(int meetingId);
    Task<RepositoryResult<Meeting?>> RemoveMeetingAsync(int meetingId);
    Task<RepositoryResult<Meeting?>> UpdateMeetingAsync(Meeting meeting);
    Task<RepositoryResult<MeetingDeleteRecoDto?>> GetMeetingToDeleteAsync(int meetingId);



}
