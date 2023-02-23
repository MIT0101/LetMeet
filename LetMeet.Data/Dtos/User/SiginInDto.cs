using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Dtos.User
{
    public class SiginInDto
    {

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email Address Is Required")]
        [EmailAddress(ErrorMessage = "The Email Address Field is Not a Valid Email Address.")]
        public string emailAddress { get; set; }

        [Required(ErrorMessage ="Password Is Reqiured")]
        [DataType(DataType.Password)]
        [StringLength(maximumLength:255,MinimumLength =8,ErrorMessage ="Password Must Be At Least 8 Letters or 255 Max ")]
        public string password { get; set; }

        public bool rememberMe { get; set; } = false;

        public string returnUrl { get; set; } = "/";
    }
}
