namespace Identity.Entities
{
    public class Product : BaseEntity
    {
        public string Title { get; set; }
       
        public string PhotoName { get; set; }
        public string Size { get; set; }
        public decimal Price {  get; set; }
        public int StockQuantity { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public ICollection<BasketProduct> BasketProducts { get; set; }

    }
}
