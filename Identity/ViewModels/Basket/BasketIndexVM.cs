using Identity.Entities;

namespace Identity.ViewModels.Basket
{
    public class BasketIndexVM
    {
        public BasketIndexVM()
        {
            BasketProducts = new List<BasketProduct>();
        }
        public List<BasketProduct> BasketProducts { get; set; }
    }
}
