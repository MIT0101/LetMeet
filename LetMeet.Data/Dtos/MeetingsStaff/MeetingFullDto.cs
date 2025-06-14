﻿using LetMeet.Data.Entites.Meetigs;
namespace LetMeet.Data.Dtos.MeetingsStaff;

public class MeetingFullDto : MeetingTypedTaskDto<MeetingTask> {
    public DateTime created{ get; init; }
    public string studentName { get; init; }
    public string supervisorName { get; init; }

    public bool isSupervisorPresent { get; init; }

    public bool isStudentPresent { get; init; } 
    public MeetingFullDto() { 
    }
    public static MeetingFullDto GetFromMeeting(Meeting m,Guid supervisorId,Guid studentId,string supervisorName,string studentName)
    {
        return new MeetingFullDto
        {
            id = m.id,
            created = m.created,
            date = m.date,
            startHour = m.startHour,
            endHour = m.endHour,
            day = (int)m.date.DayOfWeek,
            description = m.description,
            hasTasks = (m.tasks != null && m.tasks.Count > 0),
            tasks = m.tasks,
            studentId = studentId,
            supervisorId = supervisorId,
            studentName = studentName,
            supervisorName = supervisorName,
            isSupervisorPresent=m.isSupervisorPresent,
            isStudentPresent=m.isStudentPresent

        };
    }

    public bool CanDelete(DateTime currentTime,float paddingHours) {

      return date > currentTime;
    }

    public bool CanRun(DateTime currentTime,float paddingHours)
    {
    
      return currentTime >= date && date.AddHours(endHour-startHour+paddingHours) >= currentTime;
    }
}


