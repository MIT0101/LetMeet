using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LetMeet.Data.Entites.UsersInfo;

namespace LetMeet.Data.Entites.Meetigs
{
    public class Meeting 
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required(ErrorMessage = "Student Is Required")]
        public UserInfo student { get; set; }

        [Required(ErrorMessage = "Supervisor Is Required")]
        public UserInfo supervisor { get; set; }

        [Required(ErrorMessage = "Total Meeting Time Hours Is Required.")]
        [Range(minimum:0.01,maximum:int.MaxValue,ErrorMessage = "Total Meeting Time Must Be At Least 0.01 Hour")]
        public float totalTimeHours { get; set; }

        [Required(ErrorMessage = "Reminding Meeting Time Hours Is Required.")]
        [Range(minimum: 0, maximum: int.MaxValue, ErrorMessage = "Reminding Meeting Time Must Be Positive Hour")]
        public float remindingTimeHours { get; set; }

        [Required(ErrorMessage = "Meeting Start Time Is Required.")]
        [DataType(DataType.Date,ErrorMessage = "It must be a Valid Date.")]
        public DateTime startFrom { get; set; }


        [StringLength(maximumLength: 500, MinimumLength = 4, ErrorMessage = "Meeting Description Must Be Between 4 and 500 for length")]
        public string? description { get; set; }

        public List<SubMeeting>? subMeetings { get; set; }



    }
}
