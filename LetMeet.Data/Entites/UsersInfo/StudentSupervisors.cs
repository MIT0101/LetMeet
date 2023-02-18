using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Entites.UsersInfo
{
    public class StudentSupervisors
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public UserInfo student { get; set; }

        public UserInfo supervisor { get; set; }
    }
}
