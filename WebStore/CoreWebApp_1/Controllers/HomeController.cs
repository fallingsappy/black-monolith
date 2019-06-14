using Microsoft.AspNetCore.Mvc;

namespace WebStore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(int? sectionId, int? brandId)
        {
           return View();
        }

        public IActionResult _404()
        {
            return View();
        }

        public IActionResult Blog()
        {
            return View();
        }

        public IActionResult BlogSingle()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }
    }
}