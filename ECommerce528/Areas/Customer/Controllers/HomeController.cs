using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ECommerce528.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;// = new ApplicationDbContext();

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products
                .Include(e => e.Category)
                .AsQueryable();

            // Filter

            // Pagination
            products = products.Skip(0).Take(8);

            return View(products.AsEnumerable());
        }

        public IActionResult Details([FromRoute] int id)
        {
            var product = _context.Products
                .Include(e => e.Category)
                .SingleOrDefault(e => e.Id == id);

            if (product is null) return NotFound();

            /*
             * 
             * SELECT *
             * FROM products
             * WHERE categoryId = product.categoryId
             * 
             */

            var relatedProducts = _context.Products
                .Include(e => e.Category)
                .Where(e => e.CategoryId == product.CategoryId && e.Id != id)
                .Skip(0)
                .Take(4);

            // TODO
            // 1. Select Product has contain the same name
            // 2. Select product in the same price range (+30% || -30%)

            return View(new ProductWithRelatedVM()
            {
                Product = product,
                Products = relatedProducts
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public ViewResult Welcome()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
