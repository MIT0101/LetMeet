using Alachisoft.NCache.Common.ErrorHandling;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Dtos.User;

public class AddFreeDayDto
{
    [Required(ErrorMessage = "Day is Required")]
    [Range(minimum: 0, maximum: 6)]
    public int day { get; set; }

    [Required(ErrorMessage = "Free Day Hour Start is Required")]
    [Range(minimum: 0, maximum: 23, ErrorMessage = "Free Day Hour Start Must be Between 0 and 23")]
    public int startHour { get; set; }

    [Required(ErrorMessage = "Free Day Hour End is Required")]
    [Range(minimum: 0, maximum: 24, ErrorMessage = "Free Day Hour End Must be Between 0 and 24")]
    public int endHour { get; set; }
}
