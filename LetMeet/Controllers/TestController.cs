using Microsoft.AspNetCore.Mvc;

using LetMeet.Data.Entites.UsersInfo;
using LetMeet.Data.Entites.Meetigs;
using System.ComponentModel.DataAnnotations;
using LetMeet.Repositories;
using System.Collections;
using LetMeet.Repositories.Infrastructure;
using Microsoft.AspNetCore.Identity;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using Microsoft.Extensions.Options;
using System.ComponentModel.Design;

namespace LetMeet.Controllers
{
    public class TestController : Controller
    {
        private readonly IGenericRepository<UserInfo,Guid> _userRepository;


        private readonly IOptions<RepositoryDataSettings> _settings;

        private readonly SignInManager<AppIdentityUser> _signInManager;

        private readonly IPasswordGenrationRepository _passwordGenration;
        private readonly ISelectionRepository _selectionRepository;

        public TestController(IGenericRepository<UserInfo, Guid> userRepository, IOptions<RepositoryDataSettings> settings, SignInManager<AppIdentityUser> signInManager, IPasswordGenrationRepository passwordGenration, ISelectionRepository selectionRepository)
        {
            _userRepository = userRepository;
            _settings = settings;
            _signInManager = signInManager;
            _passwordGenration = passwordGenration;
            _selectionRepository = selectionRepository;
        }
        // test free days json result

        public IActionResult DayFrees() {
            DayFree d1 = new DayFree()
            {
                day = 1,
                startHour = 2,
                endHour = 3,
            };

            DayFree d2 = new DayFree()
            {
                day = 2,
                startHour = 4,
                endHour = 5,
            };

            List<DayFree> dayes = new List<DayFree>(new DayFree[] { d1, d2 });
            return Json(new { dayes});
        }

        //test jqeury alidation
        public IActionResult formValidationJquery()
        {

            return View();
        }

        // test avalble stages
        public ActionResult stage()
        {

            return Json(new {stages= _selectionRepository.GetStages() });
        }

        [HttpGet]
        public async Task<IActionResult> passwordGen(int length=16) {
            string pass=await _passwordGenration.GenerateRandomPassword(length);
            return Json(pass);
        }

        [HttpGet]
        public IActionResult grandTime() {
        return Json(new { new DateTime(2023, 1, 25).AddDays(40).Date });
        }

        //test ovverrid
        public async Task<IActionResult> ovverride1()
        {
           var user= await _userRepository.CreateAsync(new UserInfo { fullName="use genric repo"});
            return Json(new {user });
        }
        //test validation
        public IActionResult dataAnotationValidation()
        {
            UserInfo user = new UserInfo();
            user.emailAddress = "abc";
            return Json(new {result= RepositoryValidationResult.DataAnnotationsValidation(user)});
        }
        public IActionResult state() {
            return Json(new { state= ResultState.Seccess });
        }

        //test validation 
        public IActionResult validateObject()
        {

            UserInfo user = new UserInfo();
            user.emailAddress = "abc@";
            List<ValidationResult> validationResults = RepositoryValidationResult.DataAnnotationsValidation(user).ValidationErrors;

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

            RepositoryResult<List<UserInfo>> successResult = RepositoryResult<List<UserInfo>>.SuccessResult(ResultState.Seccess,users);
            List<ValidationResult> validationErrors = new List<ValidationResult>();
            validationErrors.Add(new ValidationResult("Full Name Is Required ", new string[] { nameof(UserInfo.fullName) }));

            RepositoryResult<List<UserInfo>> failureValidtionResult = RepositoryResult<List<UserInfo>>.FailureValidationResult(validationErrors);
            return Json(new { successResult, failureValidtionResult });
        }
        //test RepositoryDataSettings for skip and take
        public async Task<IActionResult> repositorySettings()
        {
            return Json(new { _settings});
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
