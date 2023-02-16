using LetMeet.Data.Entites.Identity;
using LetMeet.Data.Entites.Meetigs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetMeet.Data.Entites.UsersInfo
{
    [Index(nameof(identityId), IsUnique = true)]
    [Index(nameof(emailAddress), IsUnique = true)]

    public class UserInfo
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [Required(ErrorMessage = "Full Name Is Required")]
        [StringLength(maximumLength: 100, MinimumLength = 4, ErrorMessage = "Full Name Must Be Between 4 and 100 for length")]
        public string fullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email Address Is Required")]
        [EmailAddress(ErrorMessage = "The Email Address Field is Not a Valid Email Address.")]
        public string emailAddress { get; set; }

        [DataType(DataType.PhoneNumber,ErrorMessage = "It must be a Valid Phone Number")]
        [StringLength(maximumLength: 50, MinimumLength = 10, ErrorMessage = "Phone Number Must Be Between 10 and 50 for length")]
        public string? phoneNumber { get; set; }

        [DataType(nameof(Stage), ErrorMessage = nameof(Stage)+ " Not a Valid " + nameof(Stage)+".")]
        [EnumDataType(typeof(UserPostion), ErrorMessage = "User Stage Is Invalid.")]

        public Stage? stage { get; set; } = Stage.Unknown;

        [Required(ErrorMessage = "Postion Is Required")]
        [EnumDataType(typeof(UserPostion),ErrorMessage ="User Postion Is Invalid.")]
        public UserPostion userPostion { get; set; } = UserPostion.Unknown;

        [DataType(DataType.ImageUrl)]
        public string? profileImage { get; set; }

        public UserInfo? supervisorOrStudent { get; set; }


        [DataType(nameof(Level), ErrorMessage = nameof(Level)+ " Not a Valid " + nameof(Level) + ".")]
        [EnumDataType(typeof(Level), ErrorMessage = "User Level Is Invalid.")]

        public Level level { get; set; }= Level.Unknown;

        [Required(ErrorMessage = "User Identity Is Required")]
        public Guid identityId { get; set; }

        [Required(ErrorMessage = "User Identity Role Is Required")]
        public Guid identityRoleId { get; set; }

        [Required(ErrorMessage = "User Role Is Required")]
        [EnumDataType(typeof(UserRole),ErrorMessage ="Invalid User Role")]
        public UserRole userRole { get; set; }


        //public List<Meeting>? meets { get; set; }


        public List<DayFree>? freeDays { get; set; }

    }

    public enum Level { 
    
        High,
        Low,
        Unknown,

    }

    public enum UserPostion
    {
        Admin,
        Employee,
        Student,
        Supervisor,
        Unknown
    }

    public enum Stage
    {
        First ,
        Second ,
        Third ,
        Fourth ,
        Unknown,

    }
}
