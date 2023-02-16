using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Entites.Meetigs
{
    public class MeetingTask
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }


        [Required(ErrorMessage = "Meeting Title Is Required")]
        [StringLength(maximumLength: 500, MinimumLength = 4, ErrorMessage = "Meeting Task Must Be Between 4 and 500 for length")]
        public string title { get; set; }


        [StringLength(maximumLength: 500, MinimumLength = 4, ErrorMessage = "Meeting Task Description Must Be Between 4 and 500 for length")]
        public string decription { get; set; }


        public bool isCompleted { get; set; }=false;


    }
}
