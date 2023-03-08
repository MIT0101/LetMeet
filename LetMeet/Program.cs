global using LetMeet.Data;
global using LetMeet.Data.Entites.Identity;
global using LetMeet.Data.Entites.UsersInfo;
global using LetMeet.Repositories;
global using LetMeet.Repositories.Infrastructure;
global using LetMeet.Repositories.Repository;
using Alachisoft.NCache.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


//configure settings

//repository settings;

// working
//builder.Services.AddSingleton<RepositoryDataSettings>(new RepositoryDataSettings()
//{
//    skip = builder.Configuration.GetValue<int>("RepositoryDataSettings:skip"),
//    take = builder.Configuration.GetValue<int>("RepositoryDataSettings:take")
//});

// working
string validPasswordChars = builder.Configuration.GetValue<string>("ValidPasswordChars") ??
PasswordGenrationRepository.DefaultValidChars;

builder.Services.AddOptions<RepositoryDataSettings>()
    .Bind(builder.Configuration.GetRequiredSection(RepositoryDataSettings.NameOfSection))
    .ValidateDataAnnotations().ValidateOnStart();

//CONFFIGEURE Email Settings
builder.Services.AddOptions<EmailRepositorySettings>().Bind(builder.Configuration.GetSection(EmailRepositorySettings.SectionName))
    .ValidateDataAnnotations().ValidateOnStart();


//add identity 

builder.Services.AddIdentity<AppIdentityUser, AppIdentityRole>(options =>
{

    options.User.RequireUniqueEmail = true;

    options.Password.RequireNonAlphanumeric= false;
    options.Password.RequireDigit= false;
    options.Password.RequireLowercase= false;
    options.Password.RequireUppercase= false;
    options.Password.RequireDigit = false;

    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber= false;  
    options.Lockout.MaxFailedAccessAttempts = builder.Configuration.GetValue<int>("IdentitySettings:MaxFailedAccessAttempts");
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("IdentitySettings:DefaultLockoutTimeSpanInMinutes"));


}).AddRoles<AppIdentityRole>().AddEntityFrameworkStores<MainIdentityDbContext>()
.AddTokenProvider<DataProtectorTokenProvider<AppIdentityUser>>(TokenOptions.DefaultProvider); ;

//add db contexts

builder.Services.AddDbContext<MainIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
});

builder.Services.AddDbContext<MainDbContext>(options => {
    string cacheId = "myClusteredCache";
    NCacheConfiguration.Configure(cacheId, DependencyType.SqlServer);
    NCacheConfiguration.ConfigureLogger();

    options.UseSqlServer(builder.Configuration.GetConnectionString("MainDataConnection"));
    //options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);


});

//configure the login path
builder.Services.ConfigureApplicationCookie(options => {

    options.LoginPath = "/Account/SignIn";
});


//password genration repository
builder.Services.AddSingleton<IPasswordGenrationRepository, PasswordGenrationRepository>(options =>
{
    return new PasswordGenrationRepository(validPasswordChars);
});

//Add repositores//Add repositores

//Enums Selction repository
builder.Services.AddSingleton<ISelectionRepository, SelectionRepository>();
//add error message Repository
builder.Services.AddSingleton<IErrorMessagesRepository, ErrorMessagesRepository>();
//add email service
builder.Services.AddSingleton<IEmailRepository,EmailRepository>();
//Genric Repository of user Info
builder.Services.AddScoped<IGenericRepository<UserInfo, Guid>, GenericRepository<UserInfo, Guid>>();
//add user profile repository than inhert GenricRepository
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();






var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
