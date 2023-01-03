using Microsoft.AspNetCore.Mvc;

namespace Clean.UI.Controllers;

public class HomeController : Controller
{
    public HomeController()
    { }

    public IActionResult Index()
    {
        return View();
    }
}
