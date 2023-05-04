using LetMeet.Data.Dtos.MeetingsStaff;
using LetMeet.Data.Entites.Identity;

namespace LetMeet.Data.Dtos.User;

public interface IUserProfileDto
{
    Guid userInfoId { get; set; }
    string profileImage { get; set; }
    string fullName { get; set; }
    string email { get; set; }
    string? phoneNumber { get; set; }
    UserRole role { get; set; }
    int totalMeetings { get; set; }
    int totalMissingMeetings { get; set; }
    List<MeetingSummaryDto>? currentMounthMeetings { get; set; }
}