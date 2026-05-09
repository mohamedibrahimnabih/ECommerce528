using System.ComponentModel.DataAnnotations;

namespace ECommerce528.ViewModels
{
    public class ApplicationUserVM
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string UserName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } 
        public string? Address { get; set; }

        //
    }
}
