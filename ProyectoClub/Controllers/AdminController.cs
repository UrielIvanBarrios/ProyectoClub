using Microsoft.AspNetCore.Mvc;

namespace ProyectoClub.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
