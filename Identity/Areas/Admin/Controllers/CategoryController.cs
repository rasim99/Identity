using Microsoft.AspNetCore.Mvc;
using Identity.Areas.Admin.Models.Category;
using Identity.Data;
using Identity.Entities;

namespace Identity.Areas.Admin.Controllers
{
    [Area("Admin")]
    
    public class CategoryController : Controller
    {
      private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        #region List
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.isCategoryController = true;
            ViewBag.isIndexAction = true;
            ViewBag.PageName = "Category";
			var categories=_context.Categories.ToList();
            var model=new CategoryIndexVM { Categories = categories };
            return View(model);
        }

        #endregion

        #region Create

        [HttpGet]
        public IActionResult Create()
        {
			ViewBag.PageName = "Category";
			return View();
        }

        [HttpPost]
        public IActionResult Create(CategoryCreateVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var category = _context.Categories.FirstOrDefault(c=>c.Name==model.Name);
            if(category is not null)
            {
                ModelState.AddModelError("Name","Already Exists");
                return View(model);
            }
            category=new Entities.Category { Name = model.Name };
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Update
        [HttpGet]
        public IActionResult Update(int id)
        {
            ViewBag.PageName = "Category";
            var category = _context.Categories.Find(id);
            if (category is  null) return NotFound();
            var model = new CategoryUpdateVM { Name = category.Name };
            return View(model);
        }
        [HttpPost]
        public IActionResult Update(int id ,CategoryUpdateVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var category = _context.Categories.Find(id);
            if (category is null) return NotFound();
            var existCategory=_context.Categories.FirstOrDefault(c=>c.Name==model.Name && c.Id!=id);
            if (existCategory is not null)
            {
                ModelState.AddModelError("Name","Already exists");
                return View(model);
            }
            if (category.Name != model.Name)
            {
                category.ModifiedAt = DateTime.Now;
            }
            category.Name = model.Name;
            _context.Categories.Update(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category is null) return NotFound();
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
		#endregion
	}
}
