using Microsoft.AspNetCore.Mvc;

namespace MagicT.WebTemplate.Controllers;

public class HomeController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}