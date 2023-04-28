using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetMeet.Data.Entites.UsersInfo;

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

        public string? description { get; set; }

        [Required(ErrorMessage = "Student Present is Required")]
        public bool isStudentPresent { get; set; } = false;

        [Required(ErrorMessage = "Supervisor is Required")]
        public bool isSupervisorPresent { get; set; } = false;

        public DateTime created { get; set; }=DateTime.Now;

        public List<MeetingTask>? tasks { get; set; }

        public SupervisionInfo SupervisionInfo { get; set; }

    }
}
