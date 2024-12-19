using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Identity.Areas.Admin.Models.Product
{
	public class ProductCreateVM
	{
		[Required(ErrorMessage = "Please enter Title")]
		[MinLength(3, ErrorMessage = "Please enter minimum 3 character")]
		public string Title { get; set; }


		[Required(ErrorMessage ="choose photo")]
		public IFormFile Photo { get; set; }

		[Required(ErrorMessage = "Please enter Sizes")]
		[MinLength(1, ErrorMessage = "Please enter minimum 1 character")]
		public string Size { get; set; }

		[Required(ErrorMessage = "Please enter Price")]
		[Range(20,2000,ErrorMessage ="Range is 20 => 2000")]
		public decimal Price { get; set; }

        [Required(ErrorMessage = "Please enter stock quantity")]
		[Range(1,int.MaxValue,ErrorMessage ="Must be minimum 1")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Please choose Category")]
		[Display(Name="Category ")]
		public int CategoryId { get; set; }

	
		public List<SelectListItem>? Categories { get; set; }
	}
}
