namespace ECommerce528.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string ApplicationuserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public double ProductPrice { get; set; }
        public double TotalPrice { get; set; }
    }
}
