using System.ComponentModel.DataAnnotations;

namespace Identity.ViewModels.Account
{
    public class AccountResetPasswordVM
    {
        [Required(ErrorMessage = "please enter  new paswword")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "please enter  newConfirmPaswword")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "dont compare with password")]
        public string NewConfirmPassword { get; set; }

        [Required]
        public string Email { get; set; }
        
        [Required]
        public string Token { get; set; }
    }
}
