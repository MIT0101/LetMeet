using LetMeet.Data.Dtos.User;
using LetMeet.Helpers;
using LetMeet.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LetMeet.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public readonly IErrorMessagesRepository _errorMessages;
        private readonly ISelectionRepository _selectionRepository;


        private readonly IGenericRepository<UserInfo, Guid> _userRepository;


        public ProfileController(IHttpContextAccessor contextAccessor, IErrorMessagesRepository errorMessages, IGenericRepository<UserInfo, Guid> userRepository, ISelectionRepository selectionRepository)
        {
            _contextAccessor = contextAccessor;
            _errorMessages = errorMessages;
            _userRepository = userRepository;
            _selectionRepository = selectionRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(Guid id,UserInfo userInfo)
        {

            return RedirectToAction(actionName: "EditProfile", new { id});
        }

        [HttpGet]
        [Authorize]
        [OwnerOrInRoleGuid(IdFieldName:"id",Role:"Admin")]
        public async Task<IActionResult> EditProfile(Guid id,List<string>? errors=null, List<string>? messages=null) {
            

            errors ??= new List<string>();
            messages ??= new List<string>();
            
            ViewData[ViewStringHelper.UserRoles]=_selectionRepository.GetUserRoles();
            ViewData[ViewStringHelper.UserStages] = _selectionRepository.GetStages();

            ViewData[ViewStringHelper.Errors] = errors;
            ViewData[ViewStringHelper.Messages] = messages;

            var reposResult = await _userRepository.GetByIdAsync(id);

            if (reposResult.State==ResultState.DbError) {
                messages.Add(_errorMessages.DbError());
                return View();
            }

            if (reposResult.State == ResultState.NotFound)
            {
                errors.Add(_errorMessages.SingleItemNotFound("User Not Found To Show"));
                return View();
            }

            if (reposResult.State!=ResultState.Seccess) {
                errors.Add(_errorMessages.UnExpectedError());
                return View();
            }


            return View(reposResult.Result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(Guid id)
        {
           
            throw new NotImplementedException();
        }
        private bool isAdminOrOwner(Guid requestedId,ref List<string> errors) {
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
