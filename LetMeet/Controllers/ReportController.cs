using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetMeet.Web.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ReportController : Controller
    {
        [HttpGet("/[Controller]/Admin")]
        [Authorize(Roles ="Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
