
using System;
using Microsoft.AspNetCore.Mvc;

namespace WebStore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Exception(string Message) => throw new InvalidOperationException(Message ?? "Произошла ошибка в контроллере!");

        public IActionResult Contacts()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Status(string id)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            switch (id)
            {
                default: return Content($"Status --- {id}");
                case "404": return View("Error404");
            }
        }
    }
}
