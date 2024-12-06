using Identity.Entities;
using Identity.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(AccountRegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new User
            {
                Email=model.EmailAddress,
                UserName=model.EmailAddress,
                PhoneNumber=model.PhoneNumber,
                Country=model.Country,
                City=model.City,
            };
            var result= _userManager.CreateAsync(user,model.Password).Result;
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);

                return View(model);
            }

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        } 
        
        [HttpPost]
        public IActionResult Login(AccountLoginVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = _userManager.FindByEmailAsync(model.EmailAddress).Result;
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "email or password is wrong!!");
                return View(model);
            }
           var result= _signInManager.PasswordSignInAsync(user,model.Password,false,false).Result;
            if (!result.Succeeded) 
            { 
                ModelState.AddModelError(string.Empty, "email or password is wrong!!");
                return View(model);
            }
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);
            return RedirectToAction("index","home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
           await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
