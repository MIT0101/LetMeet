using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LetMeet.Data.Entites.Identity {
    //[Index(nameof(UserInfoId), IsUnique =true)]

    public class AppIdentityUser : IdentityUser<Guid> 
    {
        [Required]
        [StringLength(maximumLength:100,MinimumLength =4,ErrorMessage ="Full Name Must Be Between 4 and 100 for length")]
        public string FullName { get; set; }

        //[Required]
        //public  Guid? UserInfoId { get; set; }=null;
    }
}
