using Microsoft.AspNetCore.Mvc;
using Identity.Data;
using Identity.ViewModels.Category;
using Identity.ViewModels.Product;
using Identity.ViewModels.Shop;

namespace Identity.Contollers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;

		public ShopController(AppDbContext context)
		{
			_context = context;
		} 

		public IActionResult Index()
        {
            var categories=_context.Categories.ToList();
            var products=_context.Products.ToList();
            var categoriesList= new List<CategoryVM>();
            var productsList = new List<ProductVM>();
            foreach (var category in categories)
            {
                var categoryVM = new CategoryVM
                {
                    Id = category.Id,
                    Name = category.Name
                };
                categoriesList.Add(categoryVM);
            }

            foreach (var product in products)
            {
                var productVM = new ProductVM
                {
                    Id = product.Id,
                    Title = product.Title,
                    PhotoName = product.PhotoName,
                    Size = product.Size,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity
                };
                productsList.Add(productVM);
            }
            var model=new ShopIndexVM { Categories = categoriesList,Products=productsList};
            return View(model);
        }
        public IActionResult GetProducts(int categoryId)
        {
            
            var products = _context.Products.ToList();
            if (categoryId > 0) products=_context.Products.Where(p=>p.CategoryId==categoryId).ToList();
            return PartialView("_ProductPartial", products);
        }

    }
}
