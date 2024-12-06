using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Identity.Areas.Admin.Models.User
{
    
    public class UserCreateVM
    {
        public UserCreateVM()
        {
            RolesIds = new List<string>();
        }
        [Required(ErrorMessage = "please enter email address ")]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "please enter  country")]
        public string Country { get; set; }

        [Required(ErrorMessage = "please enter city")]
        public string City { get; set; }

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "please enter  paswword")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required(ErrorMessage = "please enter  confirmPaswword")]
        [Compare(nameof(Password), ErrorMessage = "dont compare with password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public List<SelectListItem>? Roles { get; set; }
        public List<string>? RolesIds { get; set; }

    }
}
