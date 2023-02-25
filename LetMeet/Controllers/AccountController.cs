using LetMeet.Data.Dtos.User;
using LetMeet.Helpers;
using LetMeet.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IO.Pipelines;
using System.Security.Claims;

namespace LetMeet.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly RoleManager<AppIdentityRole> _roleManager;
        private readonly SignInManager<AppIdentityUser> _signInManager;

        private readonly ILogger<AccountController> _logger;


        private readonly IPasswordGenrationRepository _passwordGenrator;
        private readonly IGenericRepository<UserInfo, Guid> _userRepository;
        public readonly IErrorMessagesRepository _errorMessages;
        private readonly IOptions<RepositoryDataSettings> _settings;
        private readonly IEmailRepository _mailRepository;

        private readonly IWebHostEnvironment _env;


        public AccountController(IPasswordGenrationRepository passwordGenrator, UserManager<AppIdentityUser> userManager, RoleManager<AppIdentityRole> roleManager, IGenericRepository<UserInfo, Guid> userRepository, IErrorMessagesRepository errorMessages, IOptions<RepositoryDataSettings> settings, IWebHostEnvironment env, IEmailRepository mailRepository, SignInManager<AppIdentityUser> signInManager, ILogger<AccountController> logger)
        {
            _passwordGenrator = passwordGenrator;
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
            _errorMessages = errorMessages;
            _settings = settings;
            _env = env;
            _mailRepository = mailRepository;
            _signInManager = signInManager;
            _logger = logger;
        }
        public IActionResult Edit(Guid? id)
        {
            if (id is null)
            {

                return BadRequest("id Is Required For Edit");

            }
            return Json(new { id });
        }
        public IActionResult Delete(Guid? id) {
            if (id is  null) {
                return BadRequest("id Is Required For Delete");
            }
            return Json(new { id});
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SiginInDto siginInDto)
        {
            
            List<string> errors = new List<string>();
            
            ViewData[ViewStringHelper.Errors] = errors;

            if (!ModelState.IsValid)
            {
                errors.Add(_errorMessages.ValidationError());
                return View(siginInDto);
            }

            var exsistUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == siginInDto.emailAddress);

            if (exsistUser is null) {
                //user not found at all
                errors.Add("Invalid login attempt.");
                return View(siginInDto);

            }
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true

            var siginInResult =await _signInManager.PasswordSignInAsync(user: exsistUser, password: siginInDto.password, isPersistent: siginInDto.rememberMe,
                lockoutOnFailure: false);


            if (siginInResult.RequiresTwoFactor)
            {
                //return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                errors.Add("Requires Two Factor");
                return View(siginInDto);
            }

            if (siginInResult.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                //return RedirectToPage("./Lockout");
                errors.Add("User account locked out.");
                return View(siginInDto);
            }
            if (siginInResult.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return LocalRedirect(siginInDto.returnUrl);
            }


            //ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            //invalid user name or password 

            errors.Add("Invalid login attempt.");

            return View();

        }

        [HttpGet]
        public async Task<ViewResult> SignIn()
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<ViewResult> ManageUsers(int pageIndex = 1, List<string> errors = null, List<string> messages = null)
        {
            ViewData[ViewStringHelper.Errors] = errors;
            ViewData[ViewStringHelper.Messages] = messages;
            var countResult = await _userRepository.CountQueryAsync();

            if (countResult.state != ResultState.Seccess)
            {
                errors.Add(_errorMessages.DbError());
                return View();
            }

            ViewBag.totalPages = (int)Math.Ceiling(countResult.value / (double)_settings.Value.MaxResponsesPerTime);

            var repoResult = await _userRepository.QueryInRangeAsync(pageIndex);

            if (repoResult.State == ResultState.MultipleNotFound)
            {
                messages.Add(_errorMessages.MultipleItemsNotFound("NO Items Found"));
                return View();

            }
            if (repoResult.State == ResultState.DbError)
            {
                messages.Add(_errorMessages.DbError());

                return View();
            }

            var allUsers = repoResult.Result.Select(u => RegisterUserDto.GetFromUserInfo(u)).ToList();
            ViewData[ViewStringHelper.AllUsers] = allUsers;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegisterUserDto userToRegister)
        {
            var errors = new List<string>();
            var messages = new List<string>();
            if (!ModelState.IsValid)
            {
                errors = RepositoryValidationResult.DataAnnotationsValidation(userToRegister).ValidationErrors
                    .Select(e => e.ErrorMessage).ToList();
                return RedirectToAction(nameof(ManageUsers), new { errors });
            }
            var exsistUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == userToRegister.emailAddress);

            if (exsistUser is not null)
            {
                errors.Add("Email Adress is Already Used Try Another One.");
                return RedirectToAction(nameof(ManageUsers), new { errors });
            }
            var identityUser = new AppIdentityUser()
            {
                FullName = userToRegister.fullName,
                Email = userToRegister.emailAddress,
                UserName = userToRegister.emailAddress,
                PhoneNumber = userToRegister.phoneNumber,
            };
            string password = await _passwordGenrator.GenerateRandomPassword();
            IdentityResult identityResult = await _userManager.CreateAsync(identityUser, password);

            if (!identityResult.Succeeded)
            {
                errors.AddRange(identityResult.Errors.Select(x => x.Description));
                return RedirectToAction(nameof(ManageUsers), new { errors });
            }

            identityUser = await _userManager.FindByEmailAsync(userToRegister.emailAddress);

            if (identityUser is null)
            {
                errors.Add("Can't Register The User.");
                return RedirectToAction(nameof(ManageUsers), new { errors });
            }
            //var userRole = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == userToRegister.userRole.ToString());

            //if (userRole is null)
            //{
            //    await _roleManager.CreateAsync(new AppIdentityRole { Name = userToRegister.userRole.ToString() });
            //}
            var roleResult = await _userManager.AddToRoleAsync(identityUser, userToRegister.userRole.ToString());
            // add claims to user


            if (!roleResult.Succeeded)
            {
                errors.Add("Can't Register User Role.");
                return RedirectToAction(nameof(ManageUsers), new { errors });
            }

            UserInfo userInfo = new UserInfo()
            {
                fullName = userToRegister.fullName,
                emailAddress = userToRegister.emailAddress,
                phoneNumber = userToRegister.phoneNumber,

                stage = userToRegister.stage,
                userRole = userToRegister.userRole,

                identityId = identityUser.Id,
            };

            RepositoryResult<UserInfo> repoResult = await _userRepository.CreateUniqeByAsync(userInfo,
                u => u.emailAddress == userToRegister.emailAddress);

            if (repoResult.State == ResultState.ItemAlreadyExsist)
            {
                errors.Add("User is Already Exsist.");
                return RedirectToAction(nameof(ManageUsers), new { errors });
            }

            if (repoResult.State == ResultState.ValidationError)
            {
                errors.AddRange(repoResult.ValidationErrors.Select(e => e.ErrorMessage));
                return RedirectToAction(nameof(ManageUsers), new { errors });
            }
            if (repoResult.State == ResultState.Created)
            {


                var emailResult = await _mailRepository.SendEmail(recipientEmail: "alraqym050@gmail.com", subject: "Acount Created",
                     body: $"You Account Cridantional is:<br>" +
                     $"Email : {userToRegister.emailAddress} <br>" +
                     $"Password :{password} <br>" +
                     $"Please Change Your Password .");

                if (emailResult.state != ResultState.Seccess)
                {
                    //sae to file or some ware
                }
                if (_env.IsDevelopment())
                {
                    Test1.SaveAccountToFile(userToRegister.emailAddress, password);
                }
                List<Claim> claims = 
                    getUserMainClaim(userIdentityId:identityUser.Id.ToString(),userInfoId:repoResult.Result.id.ToString());

                await _userManager.AddClaimsAsync(identityUser, claims);


                messages.Add("User Created Successfully");
                return RedirectToAction(nameof(ManageUsers), new { errors, messages });
            }

            errors.Add("Can't Register The User , UnExpected Error Happen.");
            return RedirectToAction(nameof(ManageUsers), new { errors });
        }

        private List<Claim> getUserMainClaim(string userIdentityId,string userInfoId) {

            return new List<Claim>() {
                 new Claim(ClaimsNameHelper.UserInfoId,userInfoId),
                new Claim(ClaimsNameHelper.UserIdentityId,userIdentityId),
                };
        }




    }
}
