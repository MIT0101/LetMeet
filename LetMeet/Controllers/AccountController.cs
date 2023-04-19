using LetMeet.Data.Dtos.User;
using LetMeet.Helpers;
using LetMeet.Middlewares;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace LetMeet.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly RoleManager<AppIdentityRole> _roleManager;
        private readonly SignInManager<AppIdentityUser> _signInManager;

        //db repos
        private readonly IGenericRepository<UserInfo, Guid> _userRepo;
        private readonly IUserProfileRepository _userProfileRepo;


        private readonly IPasswordGenrationRepository _passwordGenrator;
        private readonly IErrorMessagesRepository _errorMessages;
        private readonly IOptions<RepositoryDataSettings> _settings;
        private readonly IEmailRepository _mailRepository;

        private readonly ISelectionRepository _selectionRepository;

        private readonly ILogger<AccountController> _logger;


        private readonly IWebHostEnvironment _env;


        public AccountController(IPasswordGenrationRepository passwordGenrator, UserManager<AppIdentityUser> userManager, RoleManager<AppIdentityRole> roleManager, IGenericRepository<UserInfo, Guid> userRepository, IErrorMessagesRepository errorMessages, IOptions<RepositoryDataSettings> settings, IWebHostEnvironment env, IEmailRepository mailRepository, SignInManager<AppIdentityUser> signInManager, ILogger<AccountController> logger, ISelectionRepository selectionRepository, IUserProfileRepository userProfileRepository)
        {
            _passwordGenrator = passwordGenrator;
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepo = userRepository;
            _errorMessages = errorMessages;
            _settings = settings;
            _env = env;
            _mailRepository = mailRepository;
            _signInManager = signInManager;
            _logger = logger;
            _selectionRepository = selectionRepository;
            _userProfileRepo = userProfileRepository;
        }


        //for update user profile image

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OwnerOrInRoleGuid(IdFieldName: "id", Role: "Admin")]
        public async Task<RedirectToActionResult> UpdateProfileImage(Guid id,IFormFile? picInput, CancellationToken cancellationToken,
            List<string> errors = null, List<string> messages = null)
        {

            InitErrorsAndMessagesForView(ref errors, ref messages);

            if (picInput is null) {
                errors.Add("No Image To Update");

                return RedirectToAction(actionName: nameof(ProfileController.EditProfile),
                        controllerName: RouteNameHelper.ProfileControllerName, new { id, errors, messages });
            }

            var imageStream = new MemoryStream();
            
            await picInput.CopyToAsync(imageStream,cancellationToken);
           

            string saveDir = Path.Combine(_env.WebRootPath, "UsersImages");

            long sizeInBytes = Request.ContentLength ?? 0;
            double sizeInKb = sizeInBytes / 1024.0;

            _logger.LogWarning($"Request Size is {sizeInKb} kb");

            var reposResult = await _userProfileRepo.UpdateProfileImageAsync(userInfoId: id, imageStream: imageStream, saveDir);

            if (reposResult.State == ResultState.ValidationError)
            {
                errors.AddRange(reposResult.ValidationErrors.Select(e => e.ErrorMessage));

            }

            if (reposResult.State != ResultState.Seccess)
            {
                errors.Add("Can Not Update Profil Image");

            }
            if (reposResult.State == ResultState.Seccess)
            {
                messages.Add("Image Updated Successfully");

            }
            errors.AddRange(reposResult.ErrorMessages);


            return RedirectToAction(actionName: nameof(ProfileController.EditProfile),
                    controllerName: RouteNameHelper.ProfileControllerName, new { id, errors, messages });

        }

        //for update user password
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OwnerOrInRoleGuid(IdFieldName: "id", "Admin")]
        public async Task<IActionResult> ChangePassword([FromRoute] Guid id, ChangePasswordDto PasswordDto,
            List<string> errors = null, List<string> messages = null)
        {
            ViewData[ViewStringHelper.Errors] = errors ?? new List<string>();
            ViewData[ViewStringHelper.Messages] = messages ?? new List<string>();


            var userIdentityId = await _userProfileRepo.GetIdentityIdAsync(id);
            if (userIdentityId.state != ResultState.Seccess)
            {
                string error = $"Can Not Found This User {id} to Change His Password";
                _logger.LogError(error);
                errors.Add(error);
                return RedirectToAction(actionName: nameof(ProfileController.EditProfile),
                    controllerName: RouteNameHelper.ProfileControllerName, new { id, errors, messages });

            }



            AppIdentityUser? identityUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userIdentityId.value);

            if (identityUser == null)
            {
                string error = "Identity User Not Found To Change His Password";
                errors.Add(error);
                _logger.LogError(error);

                return RedirectToAction(actionName: nameof(ProfileController.EditProfile),
                controllerName: RouteNameHelper.ProfileControllerName, new { id, errors, messages });
            }



            RepositoryValidationResult validationResult;

            if (User.IsInRole(UserRole.Admin.ToString()))
            {
                validationResult = RepositoryValidationResult.DataAnnotationsValidation(ChangePasswordAdminDto.GetPasswordChange(PasswordDto));
            }
            else
            {
                validationResult = RepositoryValidationResult.DataAnnotationsValidation(PasswordDto);
            }

            if (!validationResult.IsValid)
            {

                errors.AddRange(validationResult?.ValidationErrors?.Select(e => e.ErrorMessage));
                _logger.LogWarning("Invalid Data To Change Password");

                return RedirectToAction(actionName: nameof(ProfileController.EditProfile),
                controllerName: RouteNameHelper.ProfileControllerName, new { id, errors, messages });

            }



            IdentityResult identityPasswordResult;

            if (User.IsInRole(UserRole.Admin.ToString()))
            {
                //change without old password
                string passRestToken = await _userManager.GeneratePasswordResetTokenAsync(identityUser);

                if (string.IsNullOrWhiteSpace(passRestToken))
                {

                    string error = "Fiald To Genrate Password Rest Token";
                    errors.Add(error);
                    _logger.LogError(error);

                    return RedirectToAction(actionName: nameof(ProfileController.EditProfile),
                    controllerName: RouteNameHelper.ProfileControllerName, new { id, errors, messages });
                }

                identityPasswordResult = await _userManager.ResetPasswordAsync(identityUser, passRestToken, newPassword: PasswordDto.newPassword);

                if (!identityPasswordResult.Succeeded)
                {
                    string error = "Password Rest Faild";
                    errors.Add(error);
                    _logger.LogError(error);

                    return RedirectToAction(actionName: nameof(ProfileController.EditProfile),
                    controllerName: RouteNameHelper.ProfileControllerName, new { id, errors, messages });

                }


            }
            else
            {

                identityPasswordResult = await _userManager.ChangePasswordAsync(user: identityUser, currentPassword: PasswordDto.oldPassword, newPassword: PasswordDto.newPassword);

            }

            if (!identityPasswordResult.Succeeded)
            {

                string error = "May be Old Password Is Wrong Try Again";
                errors.Add(error);
                _logger.LogWarning(error);

                return RedirectToAction(actionName: nameof(ProfileController.EditProfile),
                controllerName: RouteNameHelper.ProfileControllerName, new { id, errors, messages });


            }

            string message = "Password Changed Secsessfully";
            messages.Add(message);
            _logger.LogInformation(message);

            return RedirectToAction(actionName: nameof(ProfileController.EditProfile),
            controllerName: RouteNameHelper.ProfileControllerName, new { id, errors, messages });
        }

        public async Task<IActionResult> logOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(actionName: nameof(SignIn));
        }
        public IActionResult Delete(Guid? id)
        {
            throw new NotImplementedException();
            if (id is null)
            {
                return BadRequest("id Is Required For Delete");
            }
            return Json(new { id });
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SiginInDto siginInDto, string? ReturnUrl)
        {

            List<string> errors = new List<string>();
            ViewData[ViewStringHelper.Errors] = errors;
            ReturnUrl = (string.IsNullOrWhiteSpace(ReturnUrl)) ? "/" : ReturnUrl;


            if (!ModelState.IsValid)
            {
                errors.Add(_errorMessages.ValidationError());
                return View(siginInDto);
            }

            var exsistUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == siginInDto.emailAddress);

            if (exsistUser is null)
            {
                //user not found at all
                errors.Add("Invalid login attempt.");
                return View(siginInDto);

            }
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true

            var siginInResult = await _signInManager.PasswordSignInAsync(user: exsistUser, password: siginInDto.password, isPersistent: siginInDto.rememberMe,
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
                return LocalRedirect(ReturnUrl);
            }

            //invalid user name or password 

            errors.Add("Invalid login attempt.");

            return View();

        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ViewResult> SignIn()
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return await Task.FromResult(View());
        }
        //temprory must removed !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        [AllowAnonymous]
        [HttpGet]
        public async Task<ViewResult> ManageUsers(int pageIndex = 1, List<string> errors = null, List<string> messages = null)
        {
            ViewData[ViewStringHelper.Errors] = errors ?? new List<string>();
            ViewData[ViewStringHelper.Messages] = messages ?? new List<string>();
            //to show roles and users
            ViewData[ViewStringHelper.UserStages] = _selectionRepository.GetStages();
            ViewData[ViewStringHelper.UserRoles] = _selectionRepository.GetUserRoles();

            var countResult = await _userRepo.CountQueryAsync();

            if (countResult.state != ResultState.Seccess)
            {
                errors?.Add(_errorMessages.DbError());
                return View();
            }

            ViewBag.totalPages = (int)Math.Ceiling(countResult.value / (double)_settings.Value.MaxResponsesPerTime);

            var repoResult = await _userRepo.QueryInRangeAsync(pageIndex);

            if (repoResult.State == ResultState.NotFound)
            {
                messages?.Add(_errorMessages.MultipleItemsNotFound("NO Items Found"));
                return View();

            }
            if (repoResult.State == ResultState.DbError)
            {
                messages?.Add(_errorMessages.DbError());

                return View();
            }

            var allUsers = repoResult.Result?.Select(u => RegisterUserDto.GetFromUserInfo(u)).ToList();
            ViewData[ViewStringHelper.AllUsers] = allUsers;
            return View();
        }

        //temprory must removed !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        [AllowAnonymous]
        //[Authorize(Roles = "Admin" )]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegisterUserDto userToRegister)
        {
            var errors = new List<string>();
            var messages = new List<string>();
            if (!ModelState.IsValid)
            {
                errors = RepositoryValidationResult.DataAnnotationsValidation(userToRegister).ValidationErrors
                    .Select(e => e.ErrorMessage ?? string.Empty).ToList();
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

            var roleResult = await _userManager.AddToRoleAsync(identityUser, userToRegister.userRole.ToString());


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

            RepositoryResult<UserInfo> repoResult = await _userRepo.CreateUniqeByAsync(userInfo,
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
            if (repoResult.State == ResultState.Seccess)
            {


                var emailResult = await _mailRepository.SendEmail(recipientEmail: "alraqym050@gmail.com", subject: "Acount Created",
                     body: $"You Account Cridantional is:<br>" +
                     $"Email : {userToRegister.emailAddress} <br>" +
                     $"Password :{password} <br>" +
                     $"Please Change Your Password .");

                if (emailResult.state != ResultState.Seccess)
                {
                    //save to file or some ware
                }
                if (_env.IsDevelopment())
                {
                    DevelopmentHelper.SaveAccountToFile(userToRegister.emailAddress, password, userToRegister.userRole.ToString());
                }

                // add claims to user
                List<Claim> claims =
                    getUserMainClaim(userIdentityId: identityUser.Id.ToString(),
                    userInfoId: repoResult.Result.id.ToString(),
                    profileImage: repoResult.Result.profileImage ?? "profile_empty_img.png");

                await _userManager.AddClaimsAsync(identityUser, claims);


                messages.Add("User Created Successfully");
                return RedirectToAction(nameof(ManageUsers), new { errors, messages });
            }

            errors.Add("Can't Register The User , UnExpected Error Happen.");
            return RedirectToAction(nameof(ManageUsers), new { errors });
        }

        private List<Claim> getUserMainClaim(string userIdentityId, string userInfoId, string profileImage)
        {

            return new List<Claim>() {
                new Claim(ClaimsNameHelper.UserInfoId,userInfoId),
                new Claim(ClaimsNameHelper.UserIdentityId,userIdentityId),
                new Claim(ClaimsNameHelper.UserProfileImage,profileImage)
                };
        }

        // to init errors and messages
        private void InitErrorsAndMessagesForView(ref List<string>? errors, ref List<string>? messages)
        {

            errors ??= new List<string>();
            messages ??= new List<string>();

            ViewData[ViewStringHelper.Errors] = errors;
            ViewData[ViewStringHelper.Messages] = messages;
        }



    }
}
