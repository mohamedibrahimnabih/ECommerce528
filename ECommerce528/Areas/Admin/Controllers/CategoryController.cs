using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce528.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class CategoryController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IRepository<Category> _categoryRepository;

        public CategoryController(IRepository<Category> categoryRepository)
        {
            //_context = new();
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(int page = 1, string? query = null, CancellationToken cancellationToken = default)
        {
            //var categories = _context.Categories.AsQueryable();
            var categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken);

            // Filter
            if (query is not null)
                categories = categories.Where(e => e.Name.ToLower().Contains(query.ToLower().Trim()));

            // Pagination
            int totalPages = (int)Math.Ceiling(categories.Count() / 5.0);
            categories = categories.Skip((page - 1) * 5).Take(5);

            return View(new CategoryWithRelatedVM()
            {
                Categories = categories.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page,
                Query = query
            });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category, CancellationToken cancellationToken = default) // string name, string Description, bool status
        {
            //if(category.Name is not null && category.Name.Length > 100 && category.Name.Length < 3)

            if(!ModelState.IsValid)
                return View(category);

            //_context.Categories.Add(category);
            //_context.SaveChanges();

            await _categoryRepository.CreateAsync(category, cancellationToken);
            await _categoryRepository.CommitAsync(cancellationToken);

            //Response.Cookies.Append("success_notification", "Add Category Successfully", new()
            //{
            //    Expires = ,
            //    Domain =
            //});

            HttpContext.Session.SetString("success_notification", "Add Category Successfully");

            TempData["success_notification"] = "Add Category Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken = default)
        {
            //var category = _context.Categories.SingleOrDefault(e => e.Id == id);
            var category = await _categoryRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (category is null) return NotFound();

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Category category, CancellationToken cancellationToken = default) // string name, string Description, bool status
        {
            if (!ModelState.IsValid)
                return View(category);

            _categoryRepository.Update(category);
            await _categoryRepository.CommitAsync(cancellationToken);

            TempData["success_notification"] = "Update Category Successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (category is null) return NotFound();

            _categoryRepository.Delete(category);
            await _categoryRepository.CommitAsync(cancellationToken);

            TempData["success_notification"] = "Delete Category Successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
