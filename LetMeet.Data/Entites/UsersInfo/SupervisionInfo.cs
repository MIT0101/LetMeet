using LetMeet.Data.Entites.Meetigs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Entites.UsersInfo
{
    public class SupervisionInfo
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public UserInfo student { get; set; }

        public UserInfo supervisor { get; set; }

        [Required(ErrorMessage ="Start Date Is Required")]
        public DateTime startDate { get; set; }

        [Required(ErrorMessage = "Start Date Is Required")]
        public DateTime endDate { get; set; }

        public int extendTimes { get; set; } = 0;

        public List<Meeting>? meetings { get; set; }

    }
}
