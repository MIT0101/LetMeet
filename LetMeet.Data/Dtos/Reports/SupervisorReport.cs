using LetMeet.Data.Dtos.Supervision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Dtos.Reports;

public class SupervisorReport:ISingleReport
{
    public List<MeetingReportSummary>? EndedMeetings { get; set; } = new ();
    public List<StudentDatedSelectDto>? students{ get; set; } = new ();

    public string fullName { get; set; }
    public string email { get; set; }

}
