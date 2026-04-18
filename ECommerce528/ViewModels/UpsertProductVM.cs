namespace ECommerce528.ViewModels
{
    public class UpsertProductVM
    {
        public Product? Product { get; set; }
        public IEnumerable<ProductSubImg>? ProductSubImgs { get; set; }

        public IEnumerable<Category> Categories { get; set; } = [];
        public IEnumerable<Brand> Brands { get; set; } = [];
    }
}
