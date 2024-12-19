using System.ComponentModel.DataAnnotations;

namespace Identity.Areas.Admin.Models.Category
{
    public class CategoryCreateVM
    {
        [Required(ErrorMessage ="Please enter name")]
        [MinLength(3,ErrorMessage ="Please enter minimum 3 character")]
        public string Name { get; set; }
    }
}
