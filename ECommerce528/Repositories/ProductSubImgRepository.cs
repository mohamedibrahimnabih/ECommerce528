using ECommerce528.Repositories.IRepositories;

namespace ECommerce528.Repositories
{
    public sealed class ProductSubImgRepository : Repository<ProductSubImg>, IProductSubImgRepository
    {
        public void DeleteRange(IEnumerable<ProductSubImg> productSubImgs)
        {
            _context.ProductSubImgs.RemoveRange(productSubImgs);
        }
    }
}
