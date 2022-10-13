using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Controllers
{

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}
