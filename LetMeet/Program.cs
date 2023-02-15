global using LetMeet.Data;
global using LetMeet.Data.Entites.Identity;
using LetMeet.Data.Entites.UsersInfo;
using LetMeet.Repositories;
using LetMeet.Repositories.Infrastructure;
using LetMeet.Repositories.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


//configure settings

//repository settings

//builder.Services.Configure<RepositoryDataSettings>(builder.Configuration.GetSection("RepositoryDataSettings"));
//builder.Services.AddSingleton<RepositoryDataSettings>();
//builder.Services.Configure<RepositoryDataSettings>(options => {

//    options.skip = builder.Configuration.GetValue<int>("RepositoryDataSettings:skip");
//    options.take = builder.Configuration.GetValue<int>("RepositoryDataSettings:take");

//});
// working
builder.Services.AddSingleton<RepositoryDataSettings>(new RepositoryDataSettings()
{
    skip = builder.Configuration.GetValue<int>("RepositoryDataSettings:skip"),
    take = builder.Configuration.GetValue<int>("RepositoryDataSettings:take")
});


//add identity 

builder.Services.AddIdentity<AppIdentityUser, AppIdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber= false;  
    options.Lockout.MaxFailedAccessAttempts = builder.Configuration.GetValue<int>("IdentitySettings:MaxFailedAccessAttempts");
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("IdentitySettings:DefaultLockoutTimeSpanInMinutes"));

}).AddEntityFrameworkStores<MainIdentityDbContext>();

//add db contexts

builder.Services.AddDbContext<MainIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
});

builder.Services.AddDbContext<MainDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("MainDataConnection"));

});

//add repositores

builder.Services.AddScoped<IGenericRepository<UserInfo, Guid>, GenericRepository<UserInfo, Guid>>();





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
