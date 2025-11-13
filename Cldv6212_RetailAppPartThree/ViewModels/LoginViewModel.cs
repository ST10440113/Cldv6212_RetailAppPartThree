using System.ComponentModel.DataAnnotations;

namespace Cldv6212_RetailAppPartThree.ViewModels
{
    public class LoginViewModel
    {

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;


        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;


        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }

        public string Role { get; set; } = string.Empty;
    }
}
