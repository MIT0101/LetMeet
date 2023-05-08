using LetMeet.Data.Dtos;
using LetMeet.Data.Entites.Identity;
using LetMeet.Data.Entites.Meetigs;
using LetMeet.Data.Entites.UsersInfo;
using Microsoft.EntityFrameworkCore;


namespace LetMeet.Data
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options):base(options)
        {
        }
        public DbSet<UserInfo> UserInfos { get; set; }

        public DbSet<SupervisionInfo> SupervisionInfo { get; set; }
        public DbSet<DayFree> DayFrees { get; set; }


        public DbSet<Meeting> Meetings { get; set; }

        public DbSet<MeetingTask > MeetingTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var forkenKey in modelBuilder.Model.GetEntityTypes().SelectMany(x=>x.GetForeignKeys()))
            {
                forkenKey.DeleteBehavior = DeleteBehavior.NoAction;
            }
            // Seed the default UserInfo
            var defaultUserInfo = new UserInfo
            {
                id = Guid.Parse(DefultDbValues.DEFAULT_INFO_USER_ID),
                fullName = DefultDbValues.DEFAULT_USER_Full_Name,
                emailAddress = DefultDbValues.DEFUALT_EMAIL,
                phoneNumber = DefultDbValues.DEFAULT_USER_PHONE,
                stage = Stage.Unknown,
                userRole = UserRole.Admin,
                identityId = Guid.Parse(DefultDbValues.DEFAULT_IDENTITY_USER_ID) 
            };

            modelBuilder.Entity<UserInfo>().HasData(defaultUserInfo);
        }


    }
}
