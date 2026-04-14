namespace ECommerce528.ViewModels
{
    public class ProductWithRelatedVM
    {
        public Product Product { get; set; } = null!;
        public IEnumerable<Product> Products { get; set; } = [];
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string? Query { get; set; }
    }
}
