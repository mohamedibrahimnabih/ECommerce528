using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce528.ViewModels
{
    public class UpsertProductVM
    {
        public Product? Product { get; set; }
        public IEnumerable<ProductSubImg>? ProductSubImgs { get; set; }

        public IEnumerable<Category> Categories { get; set; } = [];
        public IEnumerable<Brand> Brands { get; set; } = [];

        public IEnumerable<SelectListItem> NewCategories { get; set; } = [];
        public IEnumerable<SelectListItem> NewBrands { get; set; } = [];
    }
}
