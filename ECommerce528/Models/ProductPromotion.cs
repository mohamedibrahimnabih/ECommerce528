using Microsoft.EntityFrameworkCore;

namespace ECommerce528.Models
{
    public class ProductPromotion
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public double Discount { get; set; } // Amazon 10
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ValidTo { get; set; } = DateTime.Now.AddDays(14);
        public bool Status { get; set; } = true;
        public int Usage { get; set; } = 100;

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
