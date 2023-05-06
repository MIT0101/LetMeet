using LetMeet.Data.Entites.UsersInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Dtos.Reports;

public class StudentReport:ISingleReport
{
    public List<MeetingReportSummary>? EndedMeetings { get; set; } = new();
    public string studentName { get; set; }

    public Stage stage { get; set; }
    public DateTime supervsionEndDate { get; set; }
    public string? supervisorName { get; set; }

    public Guid supervisorId { get; set; }

}
