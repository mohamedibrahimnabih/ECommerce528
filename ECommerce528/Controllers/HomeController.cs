using ECommerce528.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ECommerce528.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;// = new ApplicationDbContext();

        public HomeController()
        {
            _context = new();
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
