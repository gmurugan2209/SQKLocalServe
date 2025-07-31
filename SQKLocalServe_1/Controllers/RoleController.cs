using Microsoft.AspNetCore.Mvc;

namespace SQKLocalServe_1.Controllers;

public class RoleController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}