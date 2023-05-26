global using LetMeet.Data;
global using LetMeet.Data.Entites.Identity;
global using LetMeet.Data.Entites.UsersInfo;
global using LetMeet.Repositories;
global using LetMeet.Repositories.Infrastructure;
global using LetMeet.Repositories.Repository;
using LetMeet.Configure;
using LetMeet.Helpers;
using Serilog;
using System.Security.Claims;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");
try
{
    // <snip>
    var builder = WebApplication.CreateBuilder(args);

    //serilog
    builder.Host.UseSerilog((ctx, lc) => lc
       .WriteTo.Console()
       //.WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
       .WriteTo.Seq("http://localhost:5341")
       .ReadFrom.Configuration(ctx.Configuration));

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
    builder.Services.ConfigureApplicationCookie(options =>
    {

        options.LoginPath = "/Account/SignIn";
    });

    var app = builder.Build();
    //for logging
    app.UseSerilogRequestLogging(options =>
    {
        // Attach additional properties to the request completion event
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("RequestId", httpContext.TraceIdentifier); // Log request ID
            // Get the user from the HttpContext
            var user = httpContext.User;

            if (user != null && user.Identity.IsAuthenticated)
            {
                diagnosticContext.Set("UserId", user.FindFirstValue(ClaimTypes.NameIdentifier));
                diagnosticContext.Set("UserInfoId", user.FindFirstValue(ClaimsNameHelper.UserInfoId));


                // Get the user's roles
                var roles = user.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToList();
                if (roles.Any())
                {
                    diagnosticContext.Set("Roles", string.Join(", ", roles));
                }
            }
        };

    });
    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    //create user profile direcorty

    string ProfileImgaesDir = Path.Combine(app.Environment.WebRootPath, "UsersImages");

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

}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}