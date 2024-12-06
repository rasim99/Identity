using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [Authorize(Roles ="Director, Seller")]
    public class SaleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
