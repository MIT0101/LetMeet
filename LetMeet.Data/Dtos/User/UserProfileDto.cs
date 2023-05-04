using LetMeet.Data.Dtos.MeetingsStaff;
using LetMeet.Data.Dtos.Supervision;
using LetMeet.Data.Entites.Identity;
using LetMeet.Data.Entites.Meetigs;
using LetMeet.Data.Entites.UsersInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Dtos.User;

public class StudentProfileDto : IUserProfileDto
{
    public Guid userInfoId { get; set; }
    public string profileImage { get; set; }
    public string fullName { get; set; }

    public Stage Stage { get; set; }
    public string email { get; set; }
    public string? phoneNumber { get; set; }

    public UserRole role { get; set; }
    public int totalMeetings { get; set; }
    public int totalMissingMeetings { get; set; }
    public DateTime supervsionExpireDate { get; set; }
    public string? supervisorFullName { get; set; } = string.Empty;
    public int supervsionExtendTimes { get; set; }
    public List<MeetingSummaryDto>? currentMounthMeetings { get; set; } = new();
    public List<MeetingTask>? missingTasks { get; set; } = new();
}

public class SupervisorProfileDto : IUserProfileDto
{
    public Guid userInfoId { get; set; }
    public string fullName { get; set; }
    public string profileImage { get; set; }
    public string email { get; set; }
    public string? phoneNumber { get; set; }
    public UserRole role { get; set; }
    public int totalMeetings { get; set; }
    public int totalMissingMeetings { get; set; }
    public List<MeetingSummaryDto>? currentMounthMeetings { get; set; } = new();
    public List<StudentDatedSelectDto> allStudents { get; set; } = new();
}
