using Alachisoft.NCache.EntityFrameworkCore;
using Alachisoft.NCache.Management.ServiceControl;
using LetMeet.Business;
using LetMeet.Business.Implemintation;
using LetMeet.Business.Interfaces;
using LetMeet.Data.Entites.Meetigs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LetMeet.Configure
{
    public static class AppDependencies
    {
        //Add repositories
        public static void RegisterRepositories(this IServiceCollection services, ConfigurationManager configuration)
        {

            //Enums Selection repository
            services.AddSingleton<ISelectionRepository, SelectionRepository>();
            //add error message Repository
            services.AddSingleton<IErrorMessagesRepository, ErrorMessagesRepository>();
            //add email service
            services.AddSingleton<IEmailRepository, EmailRepository>();
            //for date time provider
            services.AddSingleton<AppTimeProvider>();

            //Generic Repository of user Info
            services.AddScoped<IGenericRepository<UserInfo, Guid>, GenericRepository<UserInfo, Guid>>();
            //Generic Repository of Supervision Info
            services.AddScoped<IGenericRepository<SupervisionInfo, int>, GenericRepository<SupervisionInfo, int>>();
            //Generic Repository of Day Free
            services.AddScoped<IGenericRepository<DayFree, int>, GenericRepository<DayFree, int>>();
            //Generic Repository of Meetings
            services.AddScoped<IGenericRepository<Meeting, int>, GenericRepository<Meeting, int>>();
            //Generic Repository of Meetings
            services.AddScoped<IGenericRepository<MeetingTask, int>, GenericRepository<MeetingTask, int>>();

            //add user profile repository 
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            //supervisions Info Repository
            services.AddScoped<ISupervisonRepository, SupervisonRepository>();
            services.AddScoped<IMeetingRepository, MeetingRepository>();

            string validPasswordChars = configuration.GetValue<string>("ValidPasswordChars") ?? PasswordGenrationRepository.DefaultValidChars;
            //password generation repository
            services.AddSingleton<IPasswordGenrationRepository, PasswordGenrationRepository>(options =>
            {
                return new PasswordGenrationRepository(validPasswordChars);
            });
        }

        //add DbConetxts
        public static void RegisterDbContexts(this IServiceCollection services, ConfigurationManager configuration)
        {

            services.AddDbContext<MainIdentityDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("IdentityConnection"));
            });

            services.AddDbContext<MainDbContext>(options =>
            {
                string cacheId = "myClusteredCache";
                NCacheConfiguration.Configure(cacheId, DependencyType.SqlServer);

                options.UseSqlServer(configuration.GetConnectionString("MainDataConnection"));
                //options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);


            });
        }

        //Register Options
        public static void RegisterOptions(this IServiceCollection services, ConfigurationManager configuration)
        {

           // for repository settings
            services.AddOptions<RepositoryDataSettings>()
                 .Bind(configuration.GetRequiredSection(RepositoryDataSettings.NameOfSection))
                 .ValidateDataAnnotations().ValidateOnStart();

            //CONFFIGEURE Email Settings
            services.AddOptions<EmailRepositorySettings>().Bind(configuration.GetSection(EmailRepositorySettings.SectionName))
                .ValidateDataAnnotations().ValidateOnStart();

            //for business
            services.AddOptions<AppServiceOptions>()
                .Bind(configuration.GetRequiredSection(AppServiceOptions.NameOfSection))
                .ValidateDataAnnotations().ValidateOnStart();
        }

        //Register Identity And Roles
        public static void RegisterIdentityWithRoles(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddIdentity<AppIdentityUser, AppIdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.Lockout.MaxFailedAccessAttempts = configuration.GetValue<int>("IdentitySettings:MaxFailedAccessAttempts");
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(configuration.GetValue<int>("IdentitySettings:DefaultLockoutTimeSpanInMinutes"));

            }).AddRoles<AppIdentityRole>().AddEntityFrameworkStores<MainIdentityDbContext>()
            .AddTokenProvider<DataProtectorTokenProvider<AppIdentityUser>>(TokenOptions.DefaultProvider);
        }
        //Register Services
        public static void RegisterServices(this IServiceCollection services) {
            services.AddScoped<ISupervisionService, SupervisionService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IMeetingService, MeetingService>();

        }



    }
}
