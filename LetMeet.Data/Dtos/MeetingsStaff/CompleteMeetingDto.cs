using Alachisoft.NCache.Common.ErrorHandling;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Dtos.MeetingsStaff
{
    public class CompleteMeetingDto
    {
        [Required(ErrorMessage = "Meeting Id is Required")]
        public int meetingId { get; set; }

        [Required(ErrorMessage = "Supervisor Id is Required")]
        public Guid supervisorId { get; set; } = Guid.Empty;

        [Required(ErrorMessage = "Student Id is Required")]
        public Guid studentId { get; set; } = Guid.Empty;

        public bool isStudentPresent { get; set; } = false;

        public bool hasTasks { get; set; } = false;

        List<MeetingTaskComplete> meetingTasks { get; set; }
    }

    public class MeetingTaskComplete
    {
        [Required(ErrorMessage ="Task id Is Required")]
        public int id { get; set; }

        public bool isCompleted { get; set; } = false;
    }
}
