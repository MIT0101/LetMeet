using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Dtos.MeetingsStaff
{
    public class MeetingSummaryDto
    {
        public int id { get; set; }
        public Guid supervisorId { get; set; }
        public Guid studentId { get; set; }
        public string? description { get; set; }
        public DateTime startDate { get; set; }
        public int startHour { get; set; }
        public int endHour { get; set; }
        public int tasksCount { get; set; } = 0;
        public string studentName { get; set; }=string.Empty;
        public string supervisorName { get; set; }=string.Empty;

        public MeetingSummaryDto(int id, string description, Guid supervisorId, Guid studentId, DateTime startDate, int startHour, int endHour, int tasksCount, string studentName, string supervisorName)
        {
            this.id = id;
            this.supervisorId = supervisorId;
            this.studentId = studentId;
            this.startDate = startDate;
            this.startHour = startHour;
            this.endHour = endHour;
            this.tasksCount = tasksCount;
            this.studentName = studentName;
            this.supervisorName = supervisorName;
        }
    }
}
