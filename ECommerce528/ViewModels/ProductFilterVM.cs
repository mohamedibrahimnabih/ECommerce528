namespace ECommerce528.ViewModels
{
    public record ProductFilterVM(string? query = null, decimal? minPrice = null, decimal? maxPrice = null, int? categoryId = null, int? brandId = null, bool lowQuantity = false);
}
