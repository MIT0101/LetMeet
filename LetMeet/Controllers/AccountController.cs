using Microsoft.AspNetCore.Mvc;

namespace LetMeet.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public async Task<ViewResult> SignIn()
        {
            return await Task.FromResult(View()) ;
        }
    }
}
