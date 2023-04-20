using LetMeet.Business.Results;
using LetMeet.Data.Dtos.Meeting;
using LetMeet.Data.Entites.Meetigs;
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

    }
}
