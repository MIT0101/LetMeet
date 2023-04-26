using LetMeet.Data.Entites.Meetigs;
namespace LetMeet.Data.Dtos.MeetingsStaff;

public class MeetingFullDto : MeetingTypedTaskDto<MeetingTask> {
    public MeetingFullDto() { 
    }
    public static MeetingFullDto GetFromMeeting(Meeting m)
    {
        return new MeetingFullDto
        {
            id = m.id,
            date = m.date,
            startHour = m.startHour,
            endHour = m.endHour,
            day = (int)m.date.DayOfWeek,
            description = m.description,
            hasTasks = (m.tasks != null && m.tasks.Count > 0),
            tasks = m.tasks,
            studentId = m.SupervisionInfo.student.id,
            supervisorId = m.SupervisionInfo.supervisor.id
        };
    }
}


