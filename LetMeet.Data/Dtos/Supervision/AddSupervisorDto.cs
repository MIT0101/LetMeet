using Alachisoft.NCache.Common.ErrorHandling;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LetMeet.Data.Dtos.Supervision
{
    public class AddSupervisorDto : SupervisionDto
    {
        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Start Date Is Required")]
        public DateTime startDate { get; set; }

        [Display(Name = "End Date")]
        [Required(ErrorMessage = "Start Date Is Required")]
        public DateTime endDate { get; set; }
    }

    public class SupervisionDto
    {
        [Display(Name = "Supervisor")]
        [Required(ErrorMessage = "Supervisor Is Required")]
        public Guid supervisorId { get; set; }

        [Display(Name = "Supervisor")]
        [Required(ErrorMessage = "Supervisor Is Required")]
        public Guid studentId { get; set; }

    }
}
