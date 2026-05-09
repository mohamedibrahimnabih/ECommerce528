namespace ECommerce528.Models
{
    public class FavoriteItem
    {
        public int Id { get; set; }
        public string ApplicationuserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
