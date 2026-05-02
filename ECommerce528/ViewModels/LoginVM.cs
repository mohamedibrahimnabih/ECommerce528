using System.ComponentModel.DataAnnotations;

namespace ECommerce528.ViewModels
{
    public class LoginVM
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "EmailORUserName")]
        public string EmailORUserName { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
