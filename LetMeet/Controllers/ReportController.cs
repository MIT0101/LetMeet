using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetMeet.Web.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
