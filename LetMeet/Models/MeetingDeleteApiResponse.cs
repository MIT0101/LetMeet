using LetMeet.Data.Dtos.MeetingsStaff;
using LetMeet.Data.Entites.Meetigs;
using LetMeet.Models;

namespace LetMeet.Web.Models
{
    public class MeetingDeleteApiResponse : IAppApiResponse<MeetingDeleteRecoDto>
    {
        public string status { get; set; } = string.Empty;
        public List<string> messages { get; set; } = new List<string>();
        public List<string> errors { get; set; } = new List<string>();
        public MeetingDeleteRecoDto? data { get; set; }
        public bool isSuccess { get; set; } = false;
        public MeetingDeleteApiResponse()
        {

        }
    }
}
