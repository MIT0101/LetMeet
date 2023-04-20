using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Entites.Meetigs
{
    public class Meeting
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required(ErrorMessage = "Total Meeting Time Is Required")]
        [Range(minimum: 0, maximum: 24, ErrorMessage = "Total Meeting Time Must be Between 0 and 24")]

        public int totalTimeHoure { get; set; }

        [Required(ErrorMessage = "Date is Required")]
        public DateTime date { get; set; }


        [Required(ErrorMessage = "Meeting Start Hour is Required")]
        [Range(minimum: 0, maximum: 23, ErrorMessage = "Meeting Start Hour Must be Between 0 and 23")]
        public int startHour { get; set; }


        [Required(ErrorMessage = "Meeting End Hour is Required")]
        [Range(minimum: 0, maximum: 24, ErrorMessage = "Meeting End Hour Must be Between 0 and 24")]

        public int endHour { get; set; }


        [StringLength(maximumLength: 500, MinimumLength = 4, ErrorMessage = "Meeting Description Must Be Between 4 and 500 for length")]
        public string? description { get; set; }

        [Required(ErrorMessage = "Present is Required")]
        public bool isPresent { get; set; } = false;


        public List<MeetingTask>? tasks { get; set; }

    }
}
