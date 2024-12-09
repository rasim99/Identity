using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Identity.Areas.Admin.Models.User
{
    public class UserUpdateVM
    {
        public UserUpdateVM()
        {
            RolesIds = new List<string>();
        }
        //[Required(ErrorMessage = "please enter email address ")]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string? EmailAddress { get; set; }

        [Required(ErrorMessage = "please enter  country")]
        public string Country { get; set; }

        [Required(ErrorMessage = "please enter city")]
        public string City { get; set; }

        public string? PhoneNumber { get; set; }

        //[Required(ErrorMessage = "please enter  paswword")]
        [DataType(DataType.Password)]
        public string ?NewPassword { get; set; }


        //[Required(ErrorMessage = "please enter  confirmPaswword")]
        [Compare(nameof(NewPassword), ErrorMessage = "dont compare with password")]
        [DataType(DataType.Password)]
        public string? NewConfirmPassword { get; set; }

        public List<SelectListItem>? Roles { get; set; }
        public List<string>? RolesIds { get; set; }
    }
}
