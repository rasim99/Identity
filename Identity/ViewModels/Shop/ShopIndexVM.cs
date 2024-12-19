using Identity.ViewModels.Category;
using Identity.ViewModels.Product;

namespace Identity.ViewModels.Shop
{
	public class ShopIndexVM
	{
        public List<CategoryVM> Categories { get; set; }
        public List<ProductVM> Products { get; set; }
    }
}
