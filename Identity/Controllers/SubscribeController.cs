using Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    public class SubscribeController : Controller
    {
        private readonly UserManager<User> _userManager;

        public SubscribeController(UserManager<User>userManager)
        {
            _userManager = userManager;
        }
        [HttpPost]
        public IActionResult Subscribe(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "wrong email address. (example : xx1234@gmail.com)";
                return RedirectToAction("index","home");
            }
            var user = _userManager.GetUserAsync(User).Result;
          
            if(user.IsSubscribe && user.Email == email)
            {
                TempData["Error"] = "already subscribed";
                return RedirectToAction("index", "home");
            }
            if (user.Email!=email)
            {
                TempData["Error"] = "dont have user with email";
                return RedirectToAction("index", "home");
            }
            user.IsSubscribe = true;
            var updateResult=_userManager.UpdateAsync(user).Result;
            if (!updateResult.Succeeded) return BadRequest();
    
            TempData["Success"] = "Successfuly subscribed";
            return RedirectToAction("index", "home");
        }
    }
}
