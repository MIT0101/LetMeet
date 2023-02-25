using LetMeet.Data.Dtos.User;
using LetMeet.Helpers;
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

        private readonly IGenericRepository<UserInfo, Guid> _userRepository;



        public ProfileController(IHttpContextAccessor contextAccessor, IErrorMessagesRepository errorMessages, IGenericRepository<UserInfo, Guid> userRepository)
        {
            _contextAccessor = contextAccessor;
            _errorMessages = errorMessages;
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile(Guid id) {
            List<string> errors= new List<string>();
            List<string> messages= new List<string>();
            ViewData[ViewStringHelper.Errors] = errors;
            ViewData[ViewStringHelper.Messages] = messages;
            Guid userInfoId ;
            if (!Guid.TryParse(_contextAccessor.HttpContext.User.FindFirstValue(ClaimsNameHelper.UserInfoId), out userInfoId)) {
                errors.Add(_errorMessages.UnExpectedError("Can Not Get Current User Id"));
                return View();
            }
            if (!_contextAccessor.HttpContext.User.IsInRole(UserRole.Admin.ToString())
                &&!userInfoId.Equals(id)) { 
            return Unauthorized();
            }
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
    }
}
