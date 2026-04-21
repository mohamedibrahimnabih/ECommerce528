using System.ComponentModel.DataAnnotations;

namespace ECommerce528.ViewModels
{
    public class UpdateBrandVM
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;

        //[FileExtensions(Extensions = ".png,.jpeg")]
        public IFormFile? Logo { get; set; } 

        public string? Description { get; set; }
        public bool Status { get; set; }
    }
}
