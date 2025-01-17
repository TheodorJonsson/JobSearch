using Microsoft.AspNetCore.Mvc;

namespace JobSearch.Controllers
{
    public class LoginController : Controller
    {
        [Route("/Login")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
