using LetMeet.Data.Dtos;
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
                forkenKey.DeleteBehavior= DeleteBehavior.NoAction;

            }
        }


    }
}
