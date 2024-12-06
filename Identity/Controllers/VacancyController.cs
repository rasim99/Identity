using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [Authorize(Roles ="Director, HR")]
    public class VacancyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
