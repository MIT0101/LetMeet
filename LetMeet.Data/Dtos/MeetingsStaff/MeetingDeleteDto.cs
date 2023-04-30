using LetMeet.Data.Entites.Meetigs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Dtos.MeetingsStaff
{
    public record MeetingDeleteRecoDto(int id, Guid supervisorId, Guid studentId, DateTime date, int startHour, int endHour);

}
