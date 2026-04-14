using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerce528.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductService _productService;

        public ProductController()
        {
            _context = new();
            _productService = new();
        }

        public IActionResult Index(ProductFilterVM productFilterVM, int page = 1)
        {
            var products = _context.Products
                                                    .Include(e=>e.Category)
                                                    .Include(e=>e.Brand)
                                                    .AsQueryable();

            // Filter
            if(productFilterVM.query is not null)
                products = products.Where(e => e.Name.ToLower().Contains(productFilterVM.query.ToLower().Trim()));

            if(productFilterVM.minPrice is not null)
                products = products.Where(e => e.Price >= productFilterVM.minPrice);

            if (productFilterVM.maxPrice is not null)
                products = products.Where(e => e.Price <= productFilterVM.maxPrice);

            if (productFilterVM.categoryId is not null)
                products = products.Where(e => e.CategoryId == productFilterVM.categoryId);

            if (productFilterVM.brandId is not null)
                products = products.Where(e => e.CategoryId == productFilterVM.brandId);

            if(productFilterVM.lowQuantity)
                products = products.OrderBy(e => e.Quantity);

            // TODO: save user filters

            // Pagination
            int totalPages = (int)Math.Ceiling(products.Count() / 5.0);
            products = products.Skip((page - 1) * 5).Take(5);

            return View(new ProductWithRelatedVM()
            {
                Products = products.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page,
                Query = productFilterVM.query
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product, IFormFile logo /*logo.png*/) // string name, string Description, bool status
        {
            if(logo is not null && logo.Length > 0)
            {
                // Save Logo in wwwroot
                var fileName = _productService.SaveImg(logo);

                if(fileName is not null)
                {
                    // Save Logo name in DB
                    product.MainImg = fileName;
                }
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var product = _context.Products.SingleOrDefault(e => e.Id == id);

            if (product is null) return NotFound();

            return View(product);
        }

        [HttpPost]
        public IActionResult Update(Product product, IFormFile? logo) // string name, string Description, bool status
        {
            var productInDb = _context.Products.AsNoTracking().SingleOrDefault(e => e.Id == product.Id);

            if (productInDb is null) return NotFound();

            if(logo is not null && logo.Length > 0)
            {
                // Save new Logo in wwwroot
                var fileName = _productService.SaveImg(logo);

                // Remove old Logo from wwwroot
                _productService.RemoveImg(productInDb.MainImg);

                // Save new Logo in DB
                if (fileName is not null) product.MainImg = fileName;
            }
            else
                product.MainImg = productInDb.MainImg;

            _context.Products.Update(product);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var product = _context.Products.SingleOrDefault(e => e.Id == id);
            
            if (product is null) return NotFound();

            // Remove old Logo from wwwroot
            _productService.RemoveImg(product.MainImg);

            _context.Products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
