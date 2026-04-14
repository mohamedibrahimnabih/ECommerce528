namespace ECommerce528.ViewModels
{
    public class BrandWithRelatedVM
    {
        public IEnumerable<Brand> Brands { get; set; } = [];

        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string? Query { get; set; }
    }
}
