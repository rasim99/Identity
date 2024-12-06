﻿using Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles ="Admin")]
    public class DashboardController : Controller
    {
        private readonly UserManager<User> _userManager;

        public DashboardController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var user =_userManager.GetUserAsync(User).Result;
            return View();
        }
    }
}
