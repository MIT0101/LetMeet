using Alachisoft.NCache.Common.ErrorHandling;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LetMeet.Data.Dtos.User
{
    public class ChangePasswordDto : ChangePasswordAdminDto
    {

        [Display(Name = "Old Password")]
        [Required(ErrorMessage = "Old Password Is Required")]
        [DataType(DataType.Password)]
        [StringLength(maximumLength: 255, MinimumLength = 8, ErrorMessage = "Old Password Must Be Between 8 and 255 for length ")]
        public string oldPassword { get; set; }


    }

    public class ChangePasswordAdminDto
    {

        [Display(Name = "New Password")]
        [Required(ErrorMessage = "New Password Is Required")]
        [DataType(DataType.Password)]
        [StringLength(maximumLength: 255, MinimumLength = 8, ErrorMessage = "New Password Must Be Between 8 and 255 for length ")]
        public string newPassword { get; set; }

        [Display(Name = "Confirm New Password")]
        [Required(ErrorMessage = "Confirm New Password Is Required")]
        [Compare(nameof(newPassword), ErrorMessage = "New Password Mismatch")]
        [StringLength(maximumLength: 255, MinimumLength = 8, ErrorMessage = "New Password Must Be Between 8 and 255 for length ")]
        public string confirmNewPassword { get; set; }

        public static ChangePasswordAdminDto GetPasswordChange(ChangePasswordDto passwordDto)
        {
            return new ChangePasswordAdminDto
            {
                newPassword = passwordDto.newPassword,

                confirmNewPassword = passwordDto.confirmNewPassword
            };
        }

    }
}
