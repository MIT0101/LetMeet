using LetMeet.Data.Dtos.User;
using LetMeet.Helpers;
using LetMeet.Middlewares;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Serilog;
using SerilogTimings;
namespace LetMeet.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly RoleManager<AppIdentityRole> _roleManager;
        private readonly SignInManager<AppIdentityUser> _signInManager;

        private readonly MainIdentityDbContext _mainIdentityDb;
        private readonly MainDbContext _mainDb;

        private readonly IGenericRepository<UserInfo, Guid> _userRepo;
        private readonly IUserProfileRepository _userProfileRepo;


        private readonly IPasswordGenrationRepository _passwordGenrator;
        private readonly IErrorMessagesRepository _errorMessages;
        private readonly IOptions<RepositoryDataSettings> _settings;
        private readonly IOptions<EmailRepositorySettings> _mailSeetings;
        private readonly IEmailRepository _mailRepository;

        private readonly ISelectionRepository _selectionRepository;

        private readonly ILogger<AccountController> _logger;


        private readonly IWebHostEnvironment _env;


        public AccountController(IPasswordGenrationRepository passwordGenrator, UserManager<AppIdentityUser> userManager
            , RoleManager<AppIdentityRole> roleManager, IGenericRepository<UserInfo, Guid> userRepository
            , IErrorMessagesRepository errorMessages, IOptions<RepositoryDataSettings> settings, IWebHostEnvironment env
            , IEmailRepository mailRepository, SignInManager<AppIdentityUser> signInManager, ILogger<AccountController> logger
            , ISelectionRepository selectionRepository, IUserProfileRepository userProfileRepository, MainIdentityDbContext mainIdentityDb
            , MainDbContext mainDb, IOptions<EmailRepositorySettings> mailSeetings)
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
            _mainIdentityDb = mainIdentityDb;
            _mainDb = mainDb;
            _mailSeetings = mailSeetings;
        }
        //AccessDenied endpoint
        [AllowAnonymous]
        public async Task<IActionResult> AccessDenied(string ReturnUrl = "")
        {
            ViewData[ViewStringHelper.ReturnUrl] = ReturnUrl;
            return View();
        }
        //UPDATE User Info
        [HttpPost("/[Controller]/EditProfile/{id}")]
        [OwnerOrInRoleGuid(IdFieldName: "id", Role: "Admin")]
        public async Task<IActionResult> EditProfile(Guid id, UserInfo userInfoReq)
        {
            List<string> errors = new List<string>();
            List<string> messages = new List<string>();
            //check if update data is valid
            if (!ModelState.IsValid)
            {
                errors.Add("Invalid User Data");
                errors = RepositoryValidationResult.DataAnnotationsValidation(userInfoReq).ValidationErrors
                   .Select(e => e.ErrorMessage ?? string.Empty).ToList();
                return RedirectToAction(actionName: nameof(ProfileController.EditProfile), RouteNameHelper.ProfileControllerName, new { id, errors, messages });

            }
            //try to get user 
            var user = (await _userRepo.GetByIdAsync(id)).Result;
            var identityUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == user.identityId);

            if (user is null || identityUser is null)
            {
                errors.Add("Can't Found User To Update");
                return RedirectToAction(actionName: nameof(ProfileController.EditProfile), RouteNameHelper.ProfileControllerName, new { id, errors, messages });
            }
            var userInfoTransction = await _mainDb.Database.BeginTransactionAsync();
            var identityTransction = await _mainIdentityDb.Database.BeginTransactionAsync();
            //update his data
            user.fullName = userInfoReq.fullName;
            user.phoneNumber = userInfoReq.phoneNumber;

            identityUser.FullName = userInfoReq.fullName;
            identityUser.PhoneNumber = userInfoReq.phoneNumber;

            //update role and stage if current user is admin
            if (GenricControllerHelper.GetUserRole(User) == UserRole.Admin)
            {
                user.stage = userInfoReq.stage;
                // check if there are user with same new email
                var userIdentityWithSameNewEmail = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == userInfoReq.emailAddress&&x.Id!=identityUser.Id);
                var userInfoWithSameNewEmail = (await _userRepo.FirstOrDefaultAsync(x => x.emailAddress == userInfoReq.emailAddress&&x.id!=userInfoReq.id)).Result;

                if (userIdentityWithSameNewEmail is not null || userInfoWithSameNewEmail is not null)
                {
                    await userInfoTransction.RollbackAsync();
                    await identityTransction.RollbackAsync();
                    errors.Add("There is User With Same New Email , Try Another Email");
                    return RedirectToAction(actionName: nameof(ProfileController.EditProfile), RouteNameHelper.ProfileControllerName, new { id, errors, messages });
                }
                //update email
                user.emailAddress = userInfoReq.emailAddress;
                //update identity 
                identityUser.Email = userInfoReq.emailAddress;
                identityUser.UserName = userInfoReq.emailAddress;
                //check if role is changed if its update it 
                bool isRoleUpdated = user.userRole == userInfoReq.userRole;
                if (user.userRole != userInfoReq.userRole)
                {
                    _logger.LogWarning("Change User Role FROM {oldRole} to {newRole}", user.userRole, userInfoReq.userRole);
                    //remove from old role and add new role
                    var removeRoleResult = await _userManager.RemoveFromRoleAsync(identityUser, user.userRole.ToString());
                    IdentityResult addRoleResult=IdentityResult.Success;
                    //check if he is not in role 
                    if (!(await _userManager.IsInRoleAsync(identityUser,userInfoReq.userRole.ToString()))) {
                        addRoleResult = await _userManager.AddToRoleAsync(identityUser, userInfoReq.userRole.ToString());
                    }

                    user.userRole = userInfoReq.userRole;
                    isRoleUpdated = removeRoleResult.Succeeded && addRoleResult.Succeeded;
                }
                if (!isRoleUpdated)
                {
                    await userInfoTransction.RollbackAsync();
                    await identityTransction.RollbackAsync();
                    errors.Add("Can't Update User Role");
                    return RedirectToAction(actionName: nameof(ProfileController.EditProfile), RouteNameHelper.ProfileControllerName, new { id, errors, messages });

                }


            }


            var repoUpdateResult = await _userRepo.UpdateAsync(user.id, user);
            var identityUpdateResult = await _userManager.UpdateAsync(identityUser);

            if (!repoUpdateResult.Success || !identityUpdateResult.Succeeded)
            {
                _logger.LogError("Can't Update User Profile ");
                await userInfoTransction.RollbackAsync();
                await identityTransction.RollbackAsync();
                errors.Add("Can't Update User ");
                return RedirectToAction(actionName: nameof(ProfileController.EditProfile), RouteNameHelper.ProfileControllerName, new { id, errors, messages });

            }
            _logger.LogInformation("Update User Profile Successfully with id : {id} , by {CurrentUserId}", user.id, GenricControllerHelper.GetUserInfoId(User));
            await userInfoTransction.CommitAsync();
            await identityTransction.CommitAsync();
            messages.Add("User Updated Successfully ");
            return RedirectToAction(actionName: nameof(ProfileController.EditProfile), RouteNameHelper.ProfileControllerName, new { id, errors, messages });

        }

        //for update user profile image
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OwnerOrInRoleGuid(IdFieldName: "id", Role: "Admin")]
        public async Task<RedirectToActionResult> UpdateProfileImage(Guid id, IFormFile? picInput, CancellationToken cancellationToken,
            List<string> errors = null, List<string> messages = null)
        {

            InitErrorsAndMessagesForView(ref errors, ref messages);

            if (picInput is null)
            {
                errors.Add("No Image To Update");

                return RedirectToAction(actionName: nameof(ProfileController.EditProfile),
                        controllerName: RouteNameHelper.ProfileControllerName, new { id, errors, messages });
            }

            var imageStream = new MemoryStream();

            await picInput.CopyToAsync(imageStream, cancellationToken);


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

            string message = "Password Changed Successfully";
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

        /******************************************----------- DELETE ACCOUNT ----------------*****************************************/
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            List<string> errors = new List<string>();
            List<string> messages = new List<string>();
            var adminId = GenricControllerHelper.GetUserInfoId(User);
            _logger.LogInformation("Try To Remove User With id : {id} ,by admin with id : {adminId} ", id, adminId);
            //get identity id and delete it from identity 
            var userIdentityId = (await _userProfileRepo.GetIdentityIdAsync(id)).value;
            if (userIdentityId is null || userIdentityId == Guid.Empty)
            {
                _logger.LogError("Cant Get UserIdentity Id To Remove user id : {id}", id);
                errors.Add("Can not Find User To Remove ");
                return RedirectToAction(nameof(ManageUsers), new { errors });
            }
            var identityUser = await _userManager.FindByIdAsync(userIdentityId.ToString());
            //check if user is admin if andmin check if there are another admins
            if (identityUser is not null && await _userManager.IsInRoleAsync(identityUser, UserRole.Admin.ToString()))
            {

                int adminsCount = (await _userManager.GetUsersInRoleAsync(UserRole.Admin.ToString())).Count;

                if (adminsCount <= 2)
                {
                    _logger.LogWarning("System Has Only 2 Admins and try to remove admin !");
                    errors.Add("Cant' Remove this Admin ,System must at least have 2 Admins ! ");
                    return RedirectToAction(nameof(ManageUsers), new { errors });
                }


            }

            if (identityUser is null)
            {
                _logger.LogError("Cant' Find Identity User From User Manager Remove user id : {id}", id);
                errors.Add("Can not Get User To Remove ");
                return RedirectToAction(nameof(ManageUsers), new { errors });

            }
            using var transction = await _mainIdentityDb.Database.BeginTransactionAsync();
            var identityRemoveResult = await _userManager.DeleteAsync(identityUser);
            if (!identityRemoveResult.Succeeded)
            {
                _logger.LogError("Can't Remove User From User Manager user id : {id}", id);
                errors.Add("Can not Remove User Some thing wrong happen");
                await transction.RollbackAsync();
                return RedirectToAction(nameof(ManageUsers), new { errors });
            }
            //delete it from user info and all relative meetings ,and delete all relative tasks
            var deleteRepoResult = await _userProfileRepo.RemoveEntireUser(id);

            if (!deleteRepoResult.Success)
            {
                _logger.LogError("Can't Remove User From User Profile Repository user id : {id}", id);
                errors.Add("Can Not Remove User From Our System");
                await transction.RollbackAsync();
                return RedirectToAction(nameof(ManageUsers), new { errors });

            }
            _logger.LogWarning("User Removed With Id : {id} by admin with id {adminId}", id, adminId);
            messages.Add("User Removed Successfully");
            await transction.CommitAsync();
            return RedirectToAction(nameof(ManageUsers), new { errors ,messages});

        }
        /******************************************----------- SigIn [Post] ----------------*****************************************/

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
        /******************************************----------- SIGIN [Get] ----------------*****************************************/

        [AllowAnonymous]
        [HttpGet]
        public async Task<ViewResult> SignIn()
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return await Task.FromResult(View());
        }
        /******************************************----------- MANAGE USERES [GET] ----------------*****************************************/

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ViewResult> ManageUsers(string name = "", int pageIndex = 1, List<string> errors = null, List<string> messages = null)
        {
            ViewData[ViewStringHelper.Errors] = errors ?? new List<string>();
            ViewData[ViewStringHelper.Messages] = messages ?? new List<string>();
            //to show roles and users
            ViewData[ViewStringHelper.UserStages] = _selectionRepository.GetStages();
            ViewData[ViewStringHelper.UserRoles] = _selectionRepository.GetUserRoles();
            ViewData[ViewStringHelper.NameToSearchField] = name;

            var countResult = await _userRepo.CountQueryAsync();

            if (countResult.state != ResultState.Seccess)
            {
                errors?.Add(_errorMessages.DbError());
                return View();
            }

            ViewBag.totalPages = (int)Math.Ceiling(countResult.value / (double)_settings.Value.MaxResponsesPerTime);
            RepositoryResult<List<UserInfo>> repoResult;
            if (string.IsNullOrWhiteSpace(name))
            {
                 repoResult = await _userRepo.QueryInRangeAsync(pageIndex);
            }
            else
            {
                repoResult=await _userRepo.QueryInRangeAsync(pageIndex,x=>x.fullName.ToLower().Replace(" ","").Contains(name.ToLower().Replace(" ", "")));
            }


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
        /******************************************----------- REGISTER USER [POST] ----------------*****************************************/

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegisterUserDto userToRegister)
        {
            var errors = new List<string>();
            var messages = new List<string>();
            InitErrorsAndMessagesForView(ref errors, ref messages);
            if (!ModelState.IsValid)
            {
                errors = RepositoryValidationResult.DataAnnotationsValidation(userToRegister).ValidationErrors
                    .Select(e => e.ErrorMessage ?? string.Empty).ToList();
                return RedirectToAction(nameof(ManageUsers), new { errors });
            }
            var exsistUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == userToRegister.emailAddress);

            if (exsistUser is not null)
            {
                errors.Add("Email Address is Already Used Try Another One.");
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
            using var transction = await _mainIdentityDb.Database.BeginTransactionAsync();
            IdentityResult identityResult = await _userManager.CreateAsync(identityUser, password);

            if (!identityResult.Succeeded)
            {
                await transction.RollbackAsync();
                errors.AddRange(identityResult.Errors.Select(x => x.Description));
                return RedirectToAction(nameof(ManageUsers), new { errors });
            }

            identityUser = await _userManager.FindByEmailAsync(userToRegister.emailAddress);

            if (identityUser is null)
            {
                await transction.RollbackAsync();
                errors.Add("Can't Register The User.");
                return RedirectToAction(nameof(ManageUsers), new { errors });
            }

            var roleResult = await _userManager.AddToRoleAsync(identityUser, userToRegister.userRole.ToString());


            if (!roleResult.Succeeded)
            {
                await transction.RollbackAsync();
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
                await transction.RollbackAsync();
                errors.Add("User is Already Exsist.");
                return RedirectToAction(nameof(ManageUsers), new { errors });
            }

            if (repoResult.State == ResultState.ValidationError)
            {
                await transction.RollbackAsync();
                errors.AddRange(repoResult.ValidationErrors.Select(e => e.ErrorMessage));
                return RedirectToAction(nameof(ManageUsers), new { errors });
            }
            if (repoResult.State == ResultState.Seccess)
            {
                using (Operation.Time("Sending Account Cridantionals for {Email}", identityUser.Email))
                {
                    string reciverEmail = userInfo.emailAddress;
                    if (_mailSeetings.Value.SendToSystemAccountEmail) {
                        reciverEmail = _mailSeetings.Value.Mail;
                    }

                    var emailResult = await _mailRepository.SendEmail(recipientEmail: reciverEmail, subject: "Account Created",
                         body: "Welcome To Let Meet System <br>"+
                         $"Your Account Cridantionals are:<br>" +
                         $"Email : {userToRegister.emailAddress} <br>" +
                         $"Password :{password} <br>" +
                         $"Please Change Your Password .");


                    if (emailResult.state != ResultState.Seccess)
                    {
                        //save to file or some ware
                    }
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

                await transction.CommitAsync();
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
