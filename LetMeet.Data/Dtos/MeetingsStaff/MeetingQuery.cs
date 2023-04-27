using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static log4net.Extended.Appender.RollingFileAppender;

namespace LetMeet.Data.Dtos.MeetingsStaff
{
    public class MeetingQuery
    {
        [Required(ErrorMessage = "Supervisor Id is Required")]
        public Guid supervisorId { get; set; }=Guid.Empty;

        [Required(ErrorMessage = "Student Id is Required")]
        public Guid studentId { get; set; }=Guid.Empty;
        [Required(ErrorMessage ="Start Date is Required")]
        public DateTime startDate { get; set; }
        [Required(ErrorMessage = "End Date is Required")]
        public DateTime endDate { get; set; }
        
            
    }
}
