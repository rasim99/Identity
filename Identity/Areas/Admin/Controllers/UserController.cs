using Identity.Areas.Admin.Models.User;
using Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Identity.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.isUserController = true;
            ViewBag.isIndexAction = true;
            var users=new List<UserVM>();
            foreach (var user in _userManager.Users.ToList())
            {
                if (!_userManager.IsInRoleAsync(user, "Admin").Result)
                {
                    users.Add(new UserVM
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Country = user.Country,
                        City = user.City,
                        PhoneNumber = user.PhoneNumber,
                        Roles = _userManager.GetRolesAsync(user).Result.ToList()
                    });
                }
           
            }
            var model = new UserIndexVM
            {
                Users = users
            };
            return View(model);
        }
   
        [HttpGet]
        public IActionResult Create()
        {
            var model = new UserCreateVM
            {
                Roles=_roleManager.Roles.Where(r=>r.Name !="Admin").Select(r=>new SelectListItem
                {
                    Text = r.Name,
                    Value=r.Id
                }).ToList(),
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Create(UserCreateVM model)
        {
            if (!ModelState.IsValid)return View(model);
            var user = new User
            {
                 Email=model.EmailAddress,
                 UserName=model.EmailAddress,
                 Country = model.Country,
                 City = model.City,
                 PhoneNumber = model.PhoneNumber,
            };
            var userResult=_userManager.CreateAsync(user,model.Password).Result;
            if (!userResult.Succeeded)
            {
                foreach (var error in userResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    return View(model);
                }
            }
            foreach (var roleId in model.RolesIds)
            {
                var role = _roleManager.FindByIdAsync(roleId).Result;
                if (role is null) 
                {
                    ModelState.AddModelError("RolesIds","have not  role with this roleid ");
                    return View(model);
                }
               var addToRoleResult= _userManager.AddToRoleAsync(user, role.Name).Result;
                if (!addToRoleResult.Succeeded)
                {
                    ModelState.AddModelError("RolesIds","cannot be add role to user"); 
                    return View(model);
                }
            }
           return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user == null) return NotFound("Have nat any user");
            var deleteResult=_userManager.DeleteAsync(user).Result;
            if (!deleteResult.Succeeded) return NotFound();
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public IActionResult Detail(string id) 
        {
            var user =_userManager.FindByIdAsync(id).Result;
            if (user == null) return NotFound("have not any user");
            var model = new UserDetailVM
            {
              Email=user.Email,
             City = user.City,
             Country = user.Country,
             PhoneNumber=user.PhoneNumber,
             
            };
            var roleResult=_userManager.GetRolesAsync(user).Result;
            model.Roles=roleResult.ToList();
            return View(model);
        }
    
    }
}
