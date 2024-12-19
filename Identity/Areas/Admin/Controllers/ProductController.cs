using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Identity.Areas.Admin.Models.Product;
using Identity.Data;
using Identity.Utilities.File;

namespace Identity.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public ProductController(AppDbContext context,IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        #region List
        public IActionResult Index()
        {
            ViewBag.isProductController = true;
            ViewBag.isIndexAction = true;
            ViewBag.PageName = "Product";
            var products = _context.Products.Include(p=>p.Category).ToList();
            var model=new ProductIndexVM { Products = products };
            return View(model);
        }
        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create() 
        {
            ViewBag.PageName = "Product";
            var model = new ProductCreateVM
            {
                Categories = _context.Categories.Select(c=>new SelectListItem
                {
                    Text = c.Name,
                    Value=c.Id.ToString()
                }).ToList()
            };
          return View(model);
        }

        [HttpPost]
        public ActionResult Create(ProductCreateVM model) 
        {
            model.Categories = _context.Categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList();

            if (!ModelState.IsValid) return View(model);

            var category = _context.Categories.Find(model.CategoryId);
            if (category is null)
            {
                ModelState.AddModelError("CategoryId", "Category not found");
                return View(model);
            }

            //photo operations
            if ( model.Photo==null)
            {
                ModelState.AddModelError("Photo","cant be null");
                return View(model);
            }

            if (!_fileService.IsImage(model.Photo.ContentType))
            {
                ModelState.AddModelError("Photo","wrong format");
                return View(model);
            }
            if (!_fileService.IsAvailableSize(model.Photo.Length))
            {
                ModelState.AddModelError("Photo","Limit overed");
                return View(model);
            }
            var photoName = _fileService.Upload(model.Photo, "StaticFiles/assets/img");

            //created product
            var product = new Entities.Product
            {
                Title = model.Title,
                PhotoName = photoName,
                Size = model.Size,
                Price = model.Price,
                CategoryId = model.CategoryId,
                StockQuantity=model.StockQuantity
            };
            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));

        }

        #endregion
        #region Delete
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product is null) return NotFound();
            _context.Products.Remove(product);
            _context.SaveChanges();
            _fileService.Delete("StaticFiles/assets/img",product.PhotoName);  
            return RedirectToAction(nameof(Index));
        }
		#endregion

		#region Update
		[HttpGet]
        public IActionResult Update(int id)
        {
            ViewBag.PageName = "Product";

            var product = _context.Products.Find(id);
            if (product is null) return NotFound();
            var model = new ProductUpdateVM
            {
                Title= product.Title,
                 PhotoName= product.PhotoName,
                 Size = product.Size,
                 Price = product.Price,
                 StockQuantity=product.StockQuantity,
                 CategoryId = product.CategoryId,
                 Categories=_context.Categories.Select(c => new SelectListItem
                 {
                     Text = c.Name,
                      Value=c.Id.ToString()
                 })
                 .ToList()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Update(int id,ProductUpdateVM model)
        { 
            model.Categories=_context.Categories.Select(c=> new SelectListItem
            {
              Text= c.Name,
              Value=c.Id.ToString()
            }).ToList();

            var product = _context.Products.Find(id);
            if (product is null) return NotFound();

            var category=_context.Categories.Find(model.CategoryId);
            if (category is null)
            {
                ModelState.AddModelError("Title","Not Found Category");
                return View(model);
            }
            model.PhotoName = product.PhotoName;

            if (!ModelState.IsValid)
            {
                return View(model); 
            }
            product.Title = model.Title;
            product.Size = model.Size;
            product.Price = model.Price;
            product.CategoryId = category.Id;
            product.StockQuantity = model.StockQuantity;


            if (model.Photo is not null)
            {
                if (!_fileService.IsImage(model.Photo.ContentType))
                {
                    ModelState.AddModelError("Photo","Wrong format");
                    return View(model);
                }
                if (!_fileService.IsAvailableSize(model.Photo.Length))
                {
                    ModelState.AddModelError("Photo","Limit overed");
                    return View(model);
                }
                // old photo deleting
                _fileService.Delete("StaticFiles/assets/img",product.PhotoName);           

                //new photo adding
                var photoName = _fileService.Upload(model.Photo, "StaticFiles/assets/img");                
                product.PhotoName = photoName;
            }
            
            product.ModifiedAt= DateTime.Now;
            _context.Products.Update(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
		#endregion
	
    }
}
