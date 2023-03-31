global using LetMeet.Data;
global using LetMeet.Data.Entites.Identity;
global using LetMeet.Data.Entites.UsersInfo;
global using LetMeet.Repositories;
global using LetMeet.Repositories.Infrastructure;
global using LetMeet.Repositories.Repository;
using Alachisoft.NCache.EntityFrameworkCore;
using LetMeet.Configure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//RegisterServices
builder.Services.RegisterServices();
//Register Identity And Roles
builder.Services.RegisterIdentityWithRoles(builder.Configuration);
//Register Options
builder.Services.RegisterOptions(builder.Configuration);
//register dbcontexts
builder.Services.RegisterDbContexts(builder.Configuration);
// register repositories
builder.Services.RegisterRepositories(builder.Configuration);


//configure the login path
builder.Services.ConfigureApplicationCookie(options => {

    options.LoginPath = "/Account/SignIn";
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//create user profile direcorty

string ProfileImgaesDir= Path.Combine(app.Environment.WebRootPath, "UsersImages");

if (!Directory.Exists(ProfileImgaesDir))
{
    Directory.CreateDirectory(ProfileImgaesDir);
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
