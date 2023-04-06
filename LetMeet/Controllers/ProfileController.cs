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



        //private readonly IGenericRepository<UserInfo, Guid> _userRepository;
        private readonly IUserProfileRepository _userProfileRepository;

        //services
        private readonly ISupervisionService _supervionService;



        public ProfileController(IHttpContextAccessor contextAccessor, IErrorMessagesRepository errorMessages, ISelectionRepository selectionRepository, IUserProfileRepository userProfileRepository, IWebHostEnvironment env, ISupervisionService supervionService)
        {
            _contextAccessor = contextAccessor;
            _errorMessages = errorMessages;
            _selectionRepository = selectionRepository;
            _userProfileRepository = userProfileRepository;
            _env = env;
            _supervionService = supervionService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveSupervisorFromStudent(Guid id,SupervisionDto removeSupervision)
        {
            List<string> errors = new();
            if (!ModelState.IsValid)
            {
                errors.AddRange(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction(actionName: nameof(ProfileController.EditProfile),
                controllerName: RouteNameHelper.ProfileControllerName, new { id = removeSupervision.studentId, errors });
            }
            List<string> messages = new();
            OneOf<SupervisionInfo, List<ValidationResult>, List<ServiceMassage>> serviceResult = 
                await _supervionService.RemoveStudentFromSupervisor(studentId: removeSupervision.studentId
                , supervisorId: removeSupervision.supervisorId);

            serviceResult.Switch(user => messages.Add("Supervisor Added Sucessfully")
               , validationErrors => errors.AddRange(validationErrors?.Select(ve => ve?.ErrorMessage))
               , servicemessages => errors.AddRange(servicemessages.Select(sm => sm.Message))
               );

            return RedirectToAction(actionName: nameof(ProfileController.EditProfile),
            controllerName: RouteNameHelper.ProfileControllerName, new { id = removeSupervision.studentId, errors, messages });
        }

        // to add student to supervisor
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSupervisorToStudent(Guid id,AddSupervisorDto addSupervion)
        {
            List<string> errors = new();
            if (!ModelState.IsValid)
            {
                errors.AddRange(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return RedirectToAction(actionName: nameof(ProfileController.EditProfile),
                controllerName: RouteNameHelper.ProfileControllerName, new { id = addSupervion.studentId, errors });
            }
            List<string> messages = new();
            OneOf<SupervisionInfo, List<ValidationResult>, List<ServiceMassage>> serviceResult = await _supervionService.AddStudentToSupervisor(studentId: addSupervion.studentId
                , supervisorId: addSupervion.supervisorId, startDate: addSupervion.startDate, endDate: addSupervion.endDate);

            serviceResult.Switch(user => messages.Add("Supervisor Added Sucessfully")
               , validationErrors => errors.AddRange(validationErrors?.Select(ve => ve?.ErrorMessage))
               , servicemessages => errors.AddRange(servicemessages.Select(sm => sm.Message))
               );

            return RedirectToAction(actionName: nameof(ProfileController.EditProfile),
            controllerName: RouteNameHelper.ProfileControllerName, new { id = addSupervion.studentId, errors, messages });
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
            ViewData[ViewStringHelper.AvailableSupervisors] = await _supervionService.GetAllAvailableSupervisorsAsync();

            InitErrorsAndMessagesForView(ref errors, ref messages);

            var reposResult = await _userProfileRepository.GetUserByIdAsync(id);

            errors.AddRange(reposResult.ValidationErrors.Select(v => v.ErrorMessage));
            errors.AddRange(reposResult.ErrorMessages);

            return View(reposResult.Result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(Guid id)
        {

            throw new NotImplementedException();
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
