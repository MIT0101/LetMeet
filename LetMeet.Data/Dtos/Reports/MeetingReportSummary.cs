using LetMeet.Data.Entites.Meetigs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Dtos.Reports;

public class MeetingReportSummary
{
    public int id { get; set; }
    public Guid supervisorId { get; set; }
    public Guid studentId { get; set; }
    public DateTime startDate { get; set; }
    public int startHour { get; set; }
    public int endHour { get; set; }
    public int tasksCount { get; set; } = 0;
    public string studentName { get; set; } = string.Empty;
    public string supervisorName { get; set; } = string.Empty;
    public bool isSupervisorPresent { get; set; } = false;
    public bool isStudentPresent { get; set; } = false;
    public List<MeetingTask>? EndedMeetingsTasks { get; set; } = new();


    public MeetingReportSummary(int id, Guid supervisorId, Guid studentId, DateTime startDate, int startHour, int endHour, List<MeetingTask>? endedMeetingsTasks, int tasksCount, string studentName, string supervisorName, bool isSupervisorPresent, bool isStudentPresent)
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
        this.isSupervisorPresent = isSupervisorPresent;
        this.isStudentPresent = isStudentPresent;
        EndedMeetingsTasks = endedMeetingsTasks;
    }
}
