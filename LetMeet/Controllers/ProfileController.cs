using LetMeet.Business.Interfaces;
using LetMeet.Business.Results;
using LetMeet.Data.Dtos.Supervision;
using LetMeet.Data.Dtos.User;
using LetMeet.Helpers;
using LetMeet.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using OneOf;
using OneOf.Types;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace LetMeet.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public readonly IErrorMessagesRepository _errorMessages;
        private readonly ISelectionRepository _selectionRepository;

        private readonly IWebHostEnvironment _env;

        private readonly IUserProfileRepository _userProfileRepository;

        //services
        private readonly ISupervisionService _supervionService;
        private readonly IProfileService _profileService;



        public ProfileController(IHttpContextAccessor contextAccessor, IErrorMessagesRepository errorMessages, ISelectionRepository selectionRepository, IUserProfileRepository userProfileRepository, IWebHostEnvironment env, ISupervisionService supervionService, IProfileService profileService)
        {
            _contextAccessor = contextAccessor;
            _errorMessages = errorMessages;
            _selectionRepository = selectionRepository;
            _userProfileRepository = userProfileRepository;
            _env = env;
            _supervionService = supervionService;
            _profileService = profileService;
        }
        //add free day 
        // remove free day
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OwnerOrInRoleGuid(IdFieldName: "id", Role: "Admin")]
        public async Task<IActionResult> AddFreeDay(Guid id, AddFreeDayDto freeDayDto)
        {
            List<string> errors = new List<string>();
            List<string> messages = new List<string>();
            if (!ModelState.IsValid)
            {
                errors.AddRange(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction(actionName: nameof(ProfileController.EditProfile), new { id, errors, messages });
            }
            var result=await _profileService.AddFreeDay(id, freeDayDto);
            result.Switch(
             freeDay =>
                messages.Add(((DayOfWeek)freeDay.day)+" Added as free Day")
             ,

             validationResults =>
                errors.AddRange(validationResults?.Select(e => e.ErrorMessage))
             ,

            serviceMassages =>
               errors.AddRange(serviceMassages.Select(m => m.Message))
             );


            return RedirectToAction(actionName: nameof(ProfileController.EditProfile), new { id, errors, messages });

        }

        // remove free day
        [HttpPost]
        [OwnerOrInRoleGuid(IdFieldName: "id", Role: "Admin")]
        public async Task<IActionResult> RemoveFreeDay(Guid id, int freeDayId)
        {
            List<string> errors = new List<string>();
            List<string> messages = new List<string>();
            if (!ModelState.IsValid)
            {
                errors.AddRange(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction(actionName: nameof(ProfileController.EditProfile), new { id, errors, messages });
            }
            var result = await _profileService.RemoveFreeDay(id, freeDayId);
            result.Switch(
             freeDay =>
                messages.Add(((DayOfWeek)freeDay.day) + " Removed from free days")
             ,

             validationResults =>
                errors.AddRange(validationResults?.Select(e => e.ErrorMessage))
             ,

            serviceMassages =>
               errors.AddRange(serviceMassages.Select(m => m.Message))
             );


            return RedirectToAction(actionName: nameof(ProfileController.EditProfile), new { id, errors, messages });

        }

        [HttpPost]
        [OwnerOrInRoleGuid(IdFieldName: "id", Role: "Admin")]
        public async Task<IActionResult> EditProfile(Guid id, UserInfo userInfo)
        {
            throw new NotImplementedException();

            return RedirectToAction(actionName: "EditProfile", new { id });
        }

        [HttpGet]
        [Authorize]
        [OwnerOrInRoleGuid(IdFieldName: "id", Role: "Admin")]
        public async Task<ViewResult> EditProfile(Guid id, List<string>? errors = null, List<string>? messages = null)
        {
            ViewData[ViewStringHelper.UserRoles] = _selectionRepository.GetUserRoles();
            ViewData[ViewStringHelper.UserStages] = _selectionRepository.GetStages();

            InitErrorsAndMessagesForView(ref errors, ref messages);

            var reposResult = await _userProfileRepository.GetUserByIdAsync(id);

            errors.AddRange(reposResult.ValidationErrors.Select(v => v.ErrorMessage));
            errors.AddRange(reposResult.ErrorMessages);


            //List<DayFree> dayes = new List<DayFree>(new DayFree[] { d1, d2 });
            //reposResult.Result.freeDays =dayes;

            return View(reposResult.Result);
        }
        // to init errors and messages
        private void InitErrorsAndMessagesForView(ref List<string>? errors, ref List<string>? messages)
        {

            errors ??= new List<string>();
            messages ??= new List<string>();

            ViewData[ViewStringHelper.Errors] = errors;
            ViewData[ViewStringHelper.Messages] = messages;
        }

        private bool isAdminOrOwner(Guid requestedId, ref List<string> errors)
        {
            Guid userInfoId;
            if (!Guid.TryParse(_contextAccessor.HttpContext.User.FindFirstValue(ClaimsNameHelper.UserInfoId), out userInfoId))
            {
                errors.Add(_errorMessages.UnExpectedError("Can Not Get Current User Id"));
                return false;
            }
            if (!_contextAccessor.HttpContext.User.IsInRole(UserRole.Admin.ToString())
                && !userInfoId.Equals(requestedId))
            {
                errors.Add(_errorMessages.UnExpectedError("You Cant Access "));

                return false;
            }
            return true;

        }
    }
}
