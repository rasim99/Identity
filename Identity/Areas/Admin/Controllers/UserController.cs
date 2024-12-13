using Identity.Areas.Admin.Models.User;
using Identity.Constants.Enums;
using Identity.Entities;
using Identity.Utilities.EmailHandler.Abstract;
using Identity.Utilities.EmailHandler.Models;
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
        private readonly IEmailService _emailService;

        public UserController(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        [HttpPost]
        public IActionResult SendDiscountEmails(string Subject, string Content)
        {
            if (string.IsNullOrEmpty(Subject) || string.IsNullOrEmpty(Content))
            {
                TempData["Error"] = "Subject and Body cannot be empty.";
                return RedirectToAction(nameof(Index));
            }

            var subscribedUsers = _userManager.Users
                .Where(x => x.IsSubscribe)
                .ToList();

            if (!subscribedUsers.Any())
            {
                TempData["Error"] = "No subscribed users found.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var user in subscribedUsers)
            {
                try
                {
                    _emailService.SendMessage(new Message( new  List<string>{user.Email},Subject,Content));
                }
                catch (Exception e)
                {
                    TempData["Error"] = $"Failed to send email to {user.Email}: {e.Message}";
                    return RedirectToAction(nameof(Index));
                }
            }

            TempData["Success"] = "Emails sent successfully to subscribed users.";
            return RedirectToAction(nameof(Index));
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

        #region Create
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
        #endregion
       
       
        #region Delete
        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user == null) return NotFound("Have nat any user");
            var deleteResult=_userManager.DeleteAsync(user).Result;
            if (!deleteResult.Succeeded) return NotFound();
            return RedirectToAction(nameof(Index));

        }
        #endregion

        #region Update
        [HttpGet]
        public IActionResult Update(string id) 
        { 
            var user=_userManager.FindByIdAsync(id).Result;
            if (user == null) return NotFound();

            List<string>rolesIds = new List<string>();
            var userRoles = _userManager.GetRolesAsync(user).Result;
            foreach (var userRole in userRoles)
            {
                var role = _roleManager.FindByNameAsync(userRole).Result;
                rolesIds.Add(role.Id);
            }
            var model = new UserUpdateVM
            {
              City=user.City,
              Country=user.Country,
               PhoneNumber=user.PhoneNumber,
                EmailAddress=user.Email,
                Roles=_roleManager.Roles.Where(x=>x.Name !=UserRoles.Admin.ToString()).Select(x=>new SelectListItem
                {
                   Text= x.Name,
                   Value=x.Id
                }).ToList(),
               RolesIds=rolesIds
            };
          return View(model);
        }

        [HttpPost]
        public IActionResult Update(string id,UserUpdateVM model) 
        {
            if (!ModelState.IsValid) return View(model);
            var user = _userManager.FindByIdAsync(id).Result;
            if (user == null) return NotFound();

            user.City = model.City;
            user.Country = model.Country;
            user.PhoneNumber = model.PhoneNumber;
            if (model.NewPassword is not null)
            {
                var passwordValidationResult = _userManager.PasswordValidators
               .Select(v=>v.ValidateAsync(_userManager,user,model.NewConfirmPassword)).ToList();

                foreach (var validationTask in passwordValidationResult)
                {
                    var result = validationTask.Result;
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty,error.Description);
                        }
                        return View(model);
                    }
                }
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.NewPassword);
            }
            if (model.EmailAddress is not null) user.Email = model.EmailAddress;

            List<string> rolesIds = new List<string>();
            var userRoles=_userManager.GetRolesAsync(user).Result;
            foreach (var userRole in userRoles)
            {
                var role=_roleManager.FindByNameAsync(userRole).Result;
                rolesIds.Add(role.Id);
            }
            var mustBeAddedRoleIds = model.RolesIds.Except(rolesIds).ToList();
            var mustBeDeletedRoleIds = rolesIds.Except(model.RolesIds).ToList();
            foreach (var roleId in mustBeAddedRoleIds)
            {
                var role = _roleManager.FindByIdAsync(roleId).Result;
                if (role is null)
                {
                    ModelState.AddModelError("RolesIds","have not role");
                    return View(model);
                }
                var result = _userManager.AddToRoleAsync(user,role.Name).Result;
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors) ModelState.AddModelError(string.Empty,error.Description);
                    return View(model);
                }
            }

            foreach (var roleId in mustBeDeletedRoleIds)
            {
                var role = _roleManager.FindByIdAsync(roleId).Result;
                if (role is null)
                {
                    ModelState.AddModelError("RolesIds", "have not role");
                    return View(model);
                }
                var result= _userManager.RemoveFromRoleAsync(user,role.Name).Result;
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);

                    return View(model);
                }
            }

            var updateResult=_userManager.UpdateAsync(user).Result;
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors) ModelState.AddModelError(string.Empty, error.Description);

                return View(model);
            }

 
            return RedirectToAction(nameof(Index));
        }

        #endregion
        #region Detail
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
        #endregion
    }
}
