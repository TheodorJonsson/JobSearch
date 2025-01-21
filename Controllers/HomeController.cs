using System.Diagnostics;
using JobSearch.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobSearch.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _contx;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor contx)
        {
            _logger = logger;
            _contx = contx;
        }
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("/About")]
        public IActionResult About()
        {
            return View();
        }

        [Route("/Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
