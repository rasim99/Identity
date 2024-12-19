namespace Identity.ViewModels.Product
{
    public class ProductVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
         
        public string PhotoName { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
