using LetMeet.Data.Entites.Identity;
using LetMeet.Data.Entites.UsersInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace LetMeet.Data.Dtos.User
{
    public class RegisterUserDto
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Full Name Is Required")]
        [StringLength(maximumLength: 100, MinimumLength = 4, ErrorMessage = "Full Name Must Be Between 4 and 100 for length")]
        public string fullName { get; set; } = string.Empty;


        [Display(Name = "Stage")]
        [DataType(nameof(Stage), ErrorMessage = nameof(Stage) + " Not a Valid " + nameof(Stage) + ".")]
        [EnumDataType(typeof(Stage), ErrorMessage = "User Stage Is Invalid.")]

        public Stage? stage { get; set; } = Stage.Unknown;

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email Address Is Required")]
        [EmailAddress(ErrorMessage = "The Email Address Field is Not a Valid Email Address.")]
        public string emailAddress { get; set; }

        [Display(Name = "Confirm Email")]
        [Compare(nameof(emailAddress), ErrorMessage = "Emails mismatch")]
        [EmailAddress(ErrorMessage = "Confirm Email Address Field is Not a Valid Email Address.")]
        public string confirmEmail { get; set; }


        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "It must be a Valid Phone Number")]
        [StringLength(maximumLength: 50, MinimumLength = 10, ErrorMessage = "Phone Number Must Be Between 10 and 50 for length")]
        public string? phoneNumber { get; set; }

        [Required(ErrorMessage = "User Role Is Required")]
        [EnumDataType(typeof(UserRole), ErrorMessage = "Invalid User Role")]
        public UserRole userRole { get; set; }


        public static RegisterUserDto GetFromUserInfo(UserInfo userInfo)
        {
            return new RegisterUserDto() {
            id=userInfo.id,
            fullName= userInfo.fullName,
            emailAddress=userInfo.emailAddress,
            phoneNumber=userInfo.phoneNumber,
            stage=userInfo.stage,
            userRole=userInfo.userRole,
            };
        }

    }

    

}
