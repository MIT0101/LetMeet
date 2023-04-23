using LetMeet.Data.Entites.Meetigs;

namespace LetMeet.Models
{
    public class MeetingApiResponse : IAppApiResponse<Meeting>
    {
        public string status { get; set; } = string.Empty;
        public List<string> messages { get; set; }=new List<string>();
        public List<string> errors { get; set; } = new List<string>();
        public Meeting? data { get; set; }
        public bool isSuccess { get; set; } = false;
        public MeetingApiResponse()
        {

        }
        public static MeetingApiResponse Success(Meeting meeting,string status)
        {
            return new MeetingApiResponse
            {
                data = meeting,
                status = status,
                isSuccess = true,
            };
        }

        public static MeetingApiResponse Error(List<string> errors)
        {
            return new MeetingApiResponse { isSuccess=false,errors = errors };
        }
       
    }
}
