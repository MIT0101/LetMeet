using LetMeet.Data.Entites.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace LetMeet.Data
{
    public class MainIdentityDbContext :IdentityDbContext<AppIdentityUser, AppIdentityRole, Guid> { 
        public MainIdentityDbContext(DbContextOptions<MainIdentityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

           List<AppIdentityRole> userRoles = Enum.GetNames(typeof(UserRole)).ToList().Select(u=>new AppIdentityRole { Name=u,Id=Guid.NewGuid(),NormalizedName=u.ToString().ToUpper()}).ToList();

            builder.Entity<AppIdentityRole>().HasData(
                userRoles
                );
        }

    }
}
