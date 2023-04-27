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
using LetMeet.Data.Dtos.MeetingsStaff;
using LetMeet.Business.Interfaces;
using LetMeet.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using LetMeet.Helpers;

namespace LetMeet.Controllers
{
    public class TestController : Controller
    {
        private readonly IGenericRepository<UserInfo,Guid> _userRepository;


        private readonly IOptions<RepositoryDataSettings> _settings;

        private readonly SignInManager<AppIdentityUser> _signInManager;

        private readonly IPasswordGenrationRepository _passwordGenration;
        private readonly ISelectionRepository _selectionRepository;

        private readonly IMeetingService _meetingService;

        private readonly MainDbContext _mainDb;

        public TestController(IGenericRepository<UserInfo, Guid> userRepository, IOptions<RepositoryDataSettings> settings, SignInManager<AppIdentityUser> signInManager, IPasswordGenrationRepository passwordGenration, ISelectionRepository selectionRepository, IMeetingService meetingService, MainDbContext mianDb)
        {
            _userRepository = userRepository;
            _settings = settings;
            _signInManager = signInManager;
            _passwordGenration = passwordGenration;
            _selectionRepository = selectionRepository;
            _meetingService = meetingService;
            _mainDb = mianDb;
        }
        //Test GetUserInfoId and GetUserRole
        [HttpGet]
        public IActionResult InfoIdAndUserRole()
        {
            Guid userInfoId = GenricControllerHelper.GetUserInfoId(User);
            UserRole userRole = GenricControllerHelper.GetUserRole(User);
            //user identity id
            string identityStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "No Identity Id";
            Guid identityId= Guid.Parse(identityStr);

            return Json(new { userInfoId,role= userRole.ToString(),identityId});
        }


        //test Create Meetings Withour Go To Supervsion
        //test Create Meetings
        public async Task<IActionResult> CreateMeeting2()
        {
            Guid supervisorId = Guid.Parse("e637d108-e910-46ec-b050-08db449a41ae");//Hasan Abbas (Sun 1-6)
            Guid studentId = Guid.Parse("48be2203-c8ea-4939-b051-08db449a41ae");// ali adel

            //when its Monday its not i free time
            // 16 => sunday
            //
            DateTime date = new DateTime(2023, 4, 17);

            MeetingTaskDto task1 = new MeetingTaskDto()
            {
                title = "Task 1 Title",
                description = "Task 1 Description"
            };
            MeetingTaskDto task2 = new MeetingTaskDto()
            {
                title = "Task 2 Title",
                description = "Task 2 Description"
            };

            MeetingDto meetingDto = new MeetingDto
            {
                date = date,
                startHour = 1,
                endHour = 2,
                supervisorId = supervisorId,
                studentId = studentId,
                tasks = new List<MeetingTaskDto> { task1, task2 },
                description = "Meeting Description",
                hasTasks = true
            };
            var supervison = await _mainDb.SupervisionInfo.FirstOrDefaultAsync(x=>x.student.id == meetingDto.studentId
            && x.supervisor.id==meetingDto.supervisorId);

            List<MeetingTask> myTasks= new List<MeetingTask>();
            myTasks.AddRange(meetingDto.tasks.Select(x => new MeetingTask{title=x.title,decription=x.description }));

            Meeting meeting = new Meeting {
            date=meetingDto.date,
            startHour = meetingDto.startHour,
            endHour = meetingDto.endHour,
            tasks =myTasks,
            description=meetingDto.description,
            isPresent=false,
            totalTimeHoure=meetingDto.endHour-meetingDto.startHour,
            SupervisionInfo=supervison

            };

            await _mainDb.AddAsync(meeting);
            await _mainDb.SaveChangesAsync();


            List<string> errors = new List<string>();
            List<string> messages = new List<string>();

            //var serviceResult = await _meetingService.Create(supervisorId, meetingDto);

            //serviceResult.Switch(
            //    meeting => messages.Add("Meeting Created Successfully")
            //    , validationErrors => errors.AddRange(validationErrors.Select(x => x.ErrorMessage))
            //    , serviceMessages => errors.AddRange(serviceMessages.Select(x => x.Message)));


            return Json(new { errors, messages });
        }
     

        //test Create Meetings
        public async Task<IActionResult> Create()
        {
            Guid supervisorId = Guid.Parse("899f49cc-9af4-40f5-c58a-08db3f4ef8a8");//Hasan Abbas 2 (Sun 1-6)
            Guid studentId = Guid.Parse("3562d48c-3ccc-4bed-08a2-08db321ebfd1");// ali adel

            //when its Monday its not i free time
            // 16 => sunday
            //
            DateTime date = new DateTime(2023, 4, 17);

            MeetingTaskDto task1 = new MeetingTaskDto()
            {
                title = "Task 1 Title",
                description = "Task 1 Description"
            };
            MeetingTaskDto task2 = new MeetingTaskDto()
            {
                title = "Task 2 Title",
                description = "Task 2 Description"
            };

            MeetingDto meetingDto = new MeetingDto
            {
                date = date,
                startHour = 1,
                endHour = 2,
                supervisorId = supervisorId,
                studentId = studentId,
                tasks = new List<MeetingTaskDto> { task1, task2 },
                description = "Meeting Description",
                hasTasks = true
            };

            List<string> errors = new List<string>();
            List<string> messages = new List<string>();

            var serviceResult = await _meetingService.Create(supervisorId,meetingDto);

            serviceResult.Switch(
                meeting => messages.Add("Meeting Created Successfully")
                , validationErrors => errors.AddRange(validationErrors.Select(x => x.ErrorMessage))
                , serviceMessages => errors.AddRange(serviceMessages.Select(x => x.Message)));


            return Json(new { errors, messages });
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
