using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace LetMeet.Data.Entites.Identity
{
    public class AppIdentityRole :IdentityRole<Guid>
    {
        [StringLength(maximumLength:255,ErrorMessage ="Max Length for Description is 100 Letter")]
        public string? Description { get; set; }
    }
}
