using System.ComponentModel.DataAnnotations;

namespace Identity.Areas.Admin.ViewModels.Account
{
    public class AccountLoginVM
    {
        [Required(ErrorMessage = "please enter email address ")]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }


        [Required(ErrorMessage = "please enter  paswword")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
