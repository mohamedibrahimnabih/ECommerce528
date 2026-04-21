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
            return View(new Brand());
        }

        [HttpPost]
        public IActionResult Create(CreateBrandVM createBrandVM) 
        {
            //ModelState.Remove("logo");

            Brand brand = new()
            {
                Name = createBrandVM.Name,
                Description = createBrandVM.Description,
                Status = createBrandVM.Status
            };

            if (!ModelState.IsValid)
                return View(brand);

            if (createBrandVM.Logo is not null && createBrandVM.Logo.Length > 0)
            {
                // Save Logo in wwwroot
                var fileName = _brandService.SaveImg(createBrandVM.Logo);

                if(fileName is not null)
                {
                    // Save Logo name in DB
                    brand.Logo = fileName;
                }
            }

            _context.Brands.Add(brand);
            _context.SaveChanges();

            TempData["success_notification"] = "Add Brand Successfully";

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
        public IActionResult Update(UpdateBrandVM updateBrandVM) 
        {
            if (!ModelState.IsValid)
                return View(new Brand()
                {
                    Name = updateBrandVM.Name,
                    Description = updateBrandVM.Description,
                    Status = updateBrandVM.Status
                });

            var brand = _context.Brands.SingleOrDefault(e => e.Id == updateBrandVM.Id);

            if (brand is null) return NotFound();

            if(updateBrandVM.Logo is not null && updateBrandVM.Logo.Length > 0)
            {
                // Save new Logo in wwwroot
                var fileName = _brandService.SaveImg(updateBrandVM.Logo);

                // Remove old Logo from wwwroot
                _brandService.RemoveImg(brand.Logo);

                // Save new Logo in DB
                if (fileName is not null) brand.Logo = fileName;
            }
            else
                brand.Logo = brand.Logo;

            _context.Brands.Update(brand);
            _context.SaveChanges();

            TempData["success_notification"] = "Update Brand Successfully";

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

            TempData["success_notification"] = "Remove Brand Successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
