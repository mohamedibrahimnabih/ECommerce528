namespace ECommerce528.ViewModels
{
    public class OrderWithRelatedVM
    {
        public IEnumerable<Order> Orders { get; set; } = [];

        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string? Query { get; set; }
    }
}
