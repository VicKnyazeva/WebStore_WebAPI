
using Microsoft.AspNetCore.Mvc;

namespace WebStore.Controllers
{
    public class BlogsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Single(int id)
        {
            return View(model: id);
        }

        public IActionResult Post(int id)
        {
            return View(model: id);
        }
    }
}
