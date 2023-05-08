using LetMeet.Data.Entites.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;

namespace LetMeet.Data
{
    public class MainIdentityDbContext : IdentityDbContext<AppIdentityUser, AppIdentityRole, Guid>
    {

        public MainIdentityDbContext(DbContextOptions<MainIdentityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //all rules except admin
            List<AppIdentityRole> userRoles = Enum.GetNames(typeof(UserRole)).Where(x=>x!= UserRole.Admin.ToString()).ToList().Select((u, i) => new AppIdentityRole { Name = u, Id = Guid.NewGuid(), NormalizedName = u.ToString().ToUpper() }).ToList(); ;
            // Seed the admin role
            var adminRole = new AppIdentityRole
            {
                Id =Guid.Parse(DefultDbValues.DEFAULT_ADMIN_ROLE_ID),
                Name = UserRole.Admin.ToString(),
                NormalizedName = UserRole.Admin.ToString().ToUpper()
            };
            userRoles.Add(adminRole);

            builder.Entity<AppIdentityRole>().HasData(
                userRoles
                );
            // Seed the default user
            var defaultUser = new AppIdentityUser
            {
                Id = Guid.Parse(DefultDbValues.DEFAULT_IDENTITY_USER_ID),
                FullName=DefultDbValues.DEFAULT_USER_Full_Name,
                UserName = DefultDbValues.DEFUALT_USERNAME,
                Email = DefultDbValues.DEFUALT_EMAIL,
                EmailConfirmed = true,
                NormalizedUserName = DefultDbValues.DEFUALT_USERNAME.ToUpper(),
                NormalizedEmail = DefultDbValues.DEFUALT_EMAIL.ToUpper(),
                SecurityStamp= Guid.NewGuid().ToString()
            };

            var passwordHasher = new PasswordHasher<AppIdentityUser>();
            defaultUser.PasswordHash = passwordHasher.HashPassword(defaultUser, DefultDbValues.DEFUALT_PASSWORD);

            builder.Entity<AppIdentityUser>().HasData(defaultUser);

            // Assign the admin role to the default user
            var defaultUserRole = new IdentityUserRole<Guid>
            {
                UserId = Guid.Parse(DefultDbValues.DEFAULT_IDENTITY_USER_ID),
                RoleId = Guid.Parse(DefultDbValues.DEFAULT_ADMIN_ROLE_ID)
            };
            builder.Entity<IdentityUserRole<Guid>>().HasData(defaultUserRole);

            // Seed the default user claims
            var defaultUserRoleClaim = new IdentityUserClaim<Guid>
            {
                Id = 1,
                UserId = defaultUser.Id,
                ClaimType = ClaimTypes.Role,
                ClaimValue = UserRole.Admin.ToString(),
            };

            builder.Entity<IdentityUserClaim<Guid>>().HasData(defaultUserRoleClaim);

            var defaultUserUserInfoIdClaim = new IdentityUserClaim<Guid>
            {
                Id = 2,
                UserId = defaultUser.Id,
                ClaimType = DefultDbValues.DEFUALT_USER_INFO_ID_CLAIM_NAME,
                ClaimValue = DefultDbValues.DEFAULT_INFO_USER_ID 
            };

            builder.Entity<IdentityUserClaim<Guid>>().HasData(defaultUserUserInfoIdClaim);

        }

    }
}
