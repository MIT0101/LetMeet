using Microsoft.AspNetCore.Mvc;

using LetMeet.Data.Entites.UsersInfo;
using LetMeet.Data.Entites.Meetigs;
using System.ComponentModel.DataAnnotations;
using LetMeet.Repositories;
using System.Collections;
using LetMeet.Repositories.Infrastructure;
using Microsoft.AspNetCore.Identity;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace LetMeet.Controllers
{
    public class TestController : Controller
    {
        private readonly IGenericRepository<UserInfo,Guid> _userRepository;


        private readonly RepositoryDataSettings _settings;

        private readonly SignInManager<AppIdentityUser> _signInManager;

        public TestController(IGenericRepository<UserInfo, Guid> userRepository, RepositoryDataSettings settings, SignInManager<AppIdentityUser> signInManager)
        {
            _userRepository = userRepository;
            _settings = settings;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> ovverride1()
        {
           var user= await _userRepository.CreateAsync(new UserInfo { fullName="use genric repo"});
            return Json(new {user });
        }
        public IActionResult test1()
        {
            UserInfo user = new UserInfo();
            user.emailAddress = "abc";
            return Json(new { isvalid =Meeting.validate(user)});
        }
        public IActionResult state() {
            return Json(new { state= ResultState.Created });
        }

        //test validation 
        public IActionResult validateObject()
        {

            UserInfo user = new UserInfo();
            user.emailAddress = "abc@";
            List<ValidationResult> validationResults = Meeting.validate(user);

            ModelState.AddModelError("aa","");
            if (validationResults.Count>0) {

                //foreach (ValidationResult result in validationResults)
                //{
                //    ModelState.AddModelError(result.MemberNames.First(), result.ErrorMessage);
                //}
                //return Json(new { errorCount = ModelState.ErrorCount });
                

                return Json(new { isvalid = false, errors = validationResults.Select(error => new {pro= error.MemberNames.First(),message=error.ErrorMessage }) });

            }
            return Json(new { isvalid = true });


        }
        // test Reepo result 
        public IActionResult RepositoryResult()
        {
            List<UserInfo> users = new List<UserInfo>
            {
                new UserInfo()
                {
                    fullName = "Mohammed Al Mustafa"
                }
            };

            RepositoryResult<List<UserInfo>> successResult = RepositoryResult<List<UserInfo>>.SuccessResult(ResultState.Created,users);
            List<ValidationResult> validationErrors = new List<ValidationResult>();
            validationErrors.Add(new ValidationResult("Full Name Is Required ", new string[] { nameof(UserInfo.fullName) }));

            RepositoryResult<List<UserInfo>> failureValidtionResult = RepositoryResult<List<UserInfo>>.FailureValidationResult(validationErrors);
            return Json(new { successResult, failureValidtionResult });
        }
        //test RepositoryDataSettings for skip and take
        public async Task<IActionResult> repositorySettings()
        {
            return Json(new { _settings.skip,_settings.take });
        }

        // if repo is accessed
        public async Task< IActionResult> repository() {
            //_signInManager.SignOutAsync();
            //SignInResult signInResult = await _signInManager.PasswordSignInAsync(new AppIdentityUser(), "aa",isPersistent:false, lockoutOnFailure: true);
            await _userRepository.FirstOrDefaultAsync();
            return Json(new { isvalid = true });
        }
    }
}
