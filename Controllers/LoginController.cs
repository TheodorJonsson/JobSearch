using Microsoft.AspNetCore.Mvc;

namespace JobSearch.Controllers
{
    // WORK IN PROGRESS
    public class LoginController : Controller
    {
        [Route("/Login")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
