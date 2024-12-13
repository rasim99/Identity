using Identity.Data;
using Identity.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Controllers
{
    [Authorize]

    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var sliders = _context.Sliders.ToList();
            var slidersList = new List<SliderVM>();
            foreach (var slider in sliders)
            {
                var sliderVM = new SliderVM
                {
                    PhotoPath = slider.PhotoPath,
                    Name = slider.Name,
                    Title = slider.Title,
                    Description = slider.Description
                };
                slidersList.Add(sliderVM);
            }
            var model = new HomeIndexVM { Sliders = slidersList };
            return View(model);
        }
    }
}
