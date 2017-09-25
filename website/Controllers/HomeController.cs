using Microsoft.AspNetCore.Mvc;

namespace website.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Portfolio()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
