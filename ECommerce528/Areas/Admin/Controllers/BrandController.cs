using Microsoft.AspNetCore.Mvc;

namespace ECommerce528.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class BrandController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly BrandService _brandService;
        private readonly IRepository<Brand> _brandRepository;

        public BrandController(IRepository<Brand> brandRepository)
        {
            //_context = new();
            _brandRepository = brandRepository;
            _brandService = new();
        }

        public async Task<IActionResult> Index(int page = 1, string? query = null, CancellationToken cancellationToken = default)
        {
            //var brands = _context.Brands.AsQueryable();
            var brands = await _brandRepository.GetAsync(cancellationToken: cancellationToken);

            // Filter
            if (query is not null)
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
        public async Task<IActionResult> Create(CreateBrandVM createBrandVM, CancellationToken cancellationToken = default) 
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

            await _brandRepository.CreateAsync(brand, cancellationToken);
            await _brandRepository.CommitAsync(cancellationToken);

            TempData["success_notification"] = "Add Brand Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken = default)
        {
            //var brand = _context.Brands.SingleOrDefault(e => e.Id == id);
            var brand = await _brandRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (brand is null) return NotFound();

            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateBrandVM updateBrandVM, CancellationToken cancellationToken = default) 
        {
            if (!ModelState.IsValid)
                return View(new Brand()
                {
                    Name = updateBrandVM.Name,
                    Description = updateBrandVM.Description,
                    Status = updateBrandVM.Status
                });

            //var brand = _context.Brands.SingleOrDefault(e => e.Id == updateBrandVM.Id);
            var brand = await _brandRepository.GetOneAsync(e => e.Id == updateBrandVM.Id, 
                cancellationToken: cancellationToken);

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

            _brandRepository.Update(brand);
            await _brandRepository.CommitAsync(cancellationToken);

            TempData["success_notification"] = "Update Brand Successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            //var brand = _context.Brands.SingleOrDefault(e => e.Id == id);
            var brand = await _brandRepository.GetOneAsync(e => e.Id == id,
                cancellationToken: cancellationToken);

            if (brand is null) return NotFound();

            // Remove old Logo from wwwroot
            _brandService.RemoveImg(brand.Logo);

            _brandRepository.Delete(brand);
            await _brandRepository.CommitAsync(cancellationToken);

            TempData["success_notification"] = "Remove Brand Successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
