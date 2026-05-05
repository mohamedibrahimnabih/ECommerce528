using System.ComponentModel.DataAnnotations;

namespace ECommerce528.ViewModels
{
    public class VlidateOTPVM
    {
        public int Id { get; set; }
        [Required]
        public string OTP { get; set; } = string.Empty;
    }
}
