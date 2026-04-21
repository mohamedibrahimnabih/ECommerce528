using ECommerce528.Validations;
using System.ComponentModel.DataAnnotations;

namespace ECommerce528.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        //[MaxLength(100)]
        //[MinLength(3)]
        [CustomLength(3, 100)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Status { get; set; }
    }
}
