using System.ComponentModel.DataAnnotations;

namespace Identity.ViewModels.Account
{
    public class AccountForgotPasswordVM
    {
        [Required(ErrorMessage ="please enter email")]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
