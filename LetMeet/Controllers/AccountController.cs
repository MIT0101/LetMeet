using LetMeet.Data.Dtos.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Drawing.Printing;

namespace LetMeet.Controllers
{
    public class AccountController : Controller
    {
        private readonly IPasswordGenrationRepository _passwordGenrator;
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly RoleManager<AppIdentityRole> _roleManager;
        private readonly IGenericRepository<UserInfo, Guid> _userRepository;
        public  readonly IErrorMessagesRepository _errorMessages;
        private readonly IOptions<RepositoryDataSettings> _settings;


        public AccountController(IPasswordGenrationRepository passwordGenrator, UserManager<AppIdentityUser> userManager, RoleManager<AppIdentityRole> roleManager, IGenericRepository<UserInfo, Guid> userRepository, IErrorMessagesRepository errorMessages, IOptions<RepositoryDataSettings> settings)
        {
            _passwordGenrator = passwordGenrator;
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
            _errorMessages = errorMessages;
            _settings = settings;
        }

        [HttpGet]
        public async Task<ViewResult> SignIn()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<ViewResult> ManageUsers([FromQuery] int pageIndex=1,List<string> errors=null,string message=null) {
            ViewBag.errors=errors;
            ViewBag.message=message;
            var countResult= await _userRepository.CountQueryAsync();

            if (countResult.state!=ResultState.Seccess) {
                ViewBag.message = _errorMessages.DbError();
                return View(new List<RegisterUserDto>());
            }

            ViewBag.totalPages = (int)Math.Ceiling(countResult.value / (double)_settings.Value.MaxResponsesPerTime);

            var repoResult = await _userRepository.QueryInRangeAsync(pageIndex);

            if (repoResult.State==ResultState.MultipleNotFound) {
                ViewBag.message = _errorMessages.MultipleItemsNotFound("NO Items Found");
                return View(new List<RegisterUserDto>());
      
            }
            if (repoResult.State == ResultState.DbError)
            {
                ViewBag.message = _errorMessages.DbError();
                return View(new List<RegisterUserDto>());
            }

            var allUsers = repoResult.Result.Select(u => RegisterUserDto.GetFromUserInfo(u)).ToList();

            return View(allUsers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegisterUserDto userToRegister)
        {
            var errors=new List<string>();
            if (!ModelState.IsValid) {
                return RedirectToAction("ManageUsers");
            }
            var exsistUser= await _userManager.Users.FirstOrDefaultAsync(x=>x.Email==userToRegister.emailAddress);

            if (exsistUser is not null) {
                errors.Add("Email Adress is Already Used Try Another One.");
                ViewBag.errors = errors;
                return RedirectToAction("ManageUsers");
            }
            var identityUser = new AppIdentityUser() {
                FullName = userToRegister.fullName,
                Email = userToRegister.emailAddress,
                UserName = userToRegister.fullName.Replace(" ",""),
            };
           
           IdentityResult identityResult= await _userManager.CreateAsync(identityUser, await _passwordGenrator.GenerateRandomPassword());

            if (!identityResult.Succeeded) {
                errors.AddRange(identityResult.Errors.Select(x=>x.Description));
                ViewBag.errors = errors;
                return RedirectToAction("ManageUsers");
            }

            identityUser=await _userManager.FindByEmailAsync(userToRegister.emailAddress);

            if (identityUser is null)
            {
                errors.Add("Can't Register The User.");
                ViewBag.errors = errors;
                return RedirectToAction("ManageUsers");
            }
            var userRole = await _roleManager.Roles.FirstOrDefaultAsync(r=>r.Name==userToRegister.userRole.ToString());

            if (userRole is null) {
                await _roleManager.CreateAsync(new AppIdentityRole { Name = userToRegister.userRole.ToString() });
            }
            var roleResult = await _userManager.AddToRoleAsync(identityUser, userToRegister.userRole.ToString());

            if (!roleResult.Succeeded) {
                errors.Add("Can't Register User Role.");
                ViewBag.errors = errors;
                return RedirectToAction("ManageUsers");
            }

            UserInfo userInfo = new UserInfo() {
                fullName = userToRegister.fullName,
                emailAddress = userToRegister.emailAddress,

                stage = userToRegister.stage,
                userRole = userToRegister.userRole,

                identityId = identityUser.Id,
            };

            RepositoryResult<UserInfo> repoResult=await _userRepository.CreateUniqeByAsync(userInfo,
                u => u.emailAddress==userToRegister.emailAddress);

            if (repoResult.State == ResultState.ItemAlreadyExsist)
            {
                errors.Add("User is Already Exsist.");
                ViewBag.errors = errors;
                return RedirectToAction("ManageUsers");
            }

            if (repoResult.State==ResultState.ValidationError) {
                errors.AddRange(repoResult.ValidationErrors.Select(e=>e.ErrorMessage));
                ViewBag.errors = errors;
                return RedirectToAction("ManageUsers");
            }
            if (repoResult.State==ResultState.Created) {
                ViewBag.errors = errors;
                ViewBag.message = "User Created Successfully";
                return RedirectToAction("ManageUsers");
            }

            errors.Add("Can't Register The User , UnExpected Error Happen.");
            TempData["errors"] = errors;
            return RedirectToAction("ManageUsers");
        }


    }
}
