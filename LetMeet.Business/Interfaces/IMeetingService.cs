using LetMeet.Business.Results;
using LetMeet.Data.Dtos.MeetingsStaff;
using LetMeet.Data.Entites.Identity;
using LetMeet.Data.Entites.Meetigs;
using LetMeet.Repositories;
using OneOf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Business.Interfaces
{
    public interface IMeetingService
    {
        Task<OneOf<Meeting,IEnumerable<ValidationResult>,IEnumerable<ServiceMassage>>> Create(Guid supervisorId,MeetingDto meetingDto);
        Task<OneOf<List<MeetingFullDto>, IEnumerable<ValidationResult>, IEnumerable<ServiceMassage>>> GetMeetings(Guid currentUserId , UserRole userRole, MeetingQuery query);
        Task<OneOf<Meeting, IEnumerable<ValidationResult>, IEnumerable<ServiceMassage>>> RemoveMeeting(Guid currentUserId, UserRole userRole,int meetingId);

    }
}
