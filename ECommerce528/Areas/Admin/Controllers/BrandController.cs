using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerce528.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BrandService _brandService;

        public BrandController()
        {
            _context = new();
            _brandService = new();
        }

        public IActionResult Index(int page = 1, string? query = null)
        {
            var brands = _context.Brands.AsQueryable();

            // Filter
            if(query is not null)
                brands = brands.Where(e => e.Name.ToLower().Contains(query.ToLower().Trim()));

            // Pagination
            int totalPages = (int)Math.Ceiling(brands.Count() / 5.0);
            brands = brands.Skip((page - 1) * 5).Take(5);

            return View(new BrandWithRelatedVM()
            {
                Brands = brands.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page,
                Query = query
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Brand brand, IFormFile logo /*logo.png*/) // string name, string Description, bool status
        {
            if(logo is not null && logo.Length > 0)
            {
                // Save Logo in wwwroot
                var fileName = _brandService.SaveImg(logo);

                if(fileName is not null)
                {
                    // Save Logo name in DB
                    brand.Logo = fileName;
                }
            }

            _context.Brands.Add(brand);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var brand = _context.Brands.SingleOrDefault(e => e.Id == id);

            if (brand is null) return NotFound();

            return View(brand);
        }

        [HttpPost]
        public IActionResult Update(Brand brand, IFormFile? logo) // string name, string Description, bool status
        {
            var brandInDb = _context.Brands.AsNoTracking().SingleOrDefault(e => e.Id == brand.Id);

            if (brandInDb is null) return NotFound();

            if(logo is not null && logo.Length > 0)
            {
                // Save new Logo in wwwroot
                var fileName = _brandService.SaveImg(logo);

                // Remove old Logo from wwwroot
                _brandService.RemoveImg(brandInDb.Logo);

                // Save new Logo in DB
                if (fileName is not null) brand.Logo = fileName;
            }
            else
                brand.Logo = brandInDb.Logo;

            _context.Brands.Update(brand);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var brand = _context.Brands.SingleOrDefault(e => e.Id == id);
            
            if (brand is null) return NotFound();

            // Remove old Logo from wwwroot
            _brandService.RemoveImg(brand.Logo);

            _context.Brands.Remove(brand);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
