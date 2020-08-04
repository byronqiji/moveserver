using Microsoft.AspNetCore.Mvc;

namespace DBMoveServer.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Json(new { Value = "OK" });
        }
    }
}
