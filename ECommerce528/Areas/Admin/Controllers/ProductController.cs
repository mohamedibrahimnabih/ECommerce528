using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce528.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class ProductController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly ProductService _productService;

        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Brand> _brandRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IProductSubImgRepository _productSubImgRepository;

        public ProductController(IRepository<Category> categoryRepository, IRepository<Brand> brandRepository, IRepository<Product> productRepository, IProductSubImgRepository productSubImgRepository)
        {
            //_context = new();
            _productService = new();

            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _productRepository = productRepository;
            _productSubImgRepository = productSubImgRepository;
        }

        public async Task<IActionResult> Index(ProductFilterVM productFilterVM, int page = 1, CancellationToken cancellationToken = default)
        {
            //var products = _context.Products
            //                        .Include(e=>e.Category)
            //                        .Include(e=>e.Brand)
            //                        .AsQueryable();

            var products = await _productRepository.GetAsync(includes: [e => e.Category, e => e.Brand]);

            // Filter
            if (productFilterVM.query is not null)
                products = products.Where(e => e.Name.ToLower().Contains(productFilterVM.query.ToLower().Trim()));

            if(productFilterVM.minPrice is not null)
            {
                products = products.Where(e => e.Price >= productFilterVM.minPrice);
                ViewBag.minPrice = productFilterVM.minPrice;
            }

            if (productFilterVM.maxPrice is not null)
            {
                products = products.Where(e => e.Price <= productFilterVM.maxPrice);
                ViewBag.maxPrice = productFilterVM.maxPrice;
            }

            if (productFilterVM.categoryId is not null)
            {
                products = products.Where(e => e.CategoryId == productFilterVM.categoryId);
                ViewBag.categoryId = productFilterVM.categoryId;
            }

            if (productFilterVM.brandId is not null)
            {
                products = products.Where(e => e.BrandId == productFilterVM.brandId);
                ViewBag.brandId = productFilterVM.brandId;
            }

            if (productFilterVM.lowQuantity)
            {
                products = products.OrderBy(e => e.Quantity);
                ViewBag.lowQuantity = productFilterVM.lowQuantity;
            }

            // Pagination
            int totalPages = (int)Math.Ceiling(products.Count() / 5.0);
            products = products.Skip((page - 1) * 5).Take(5);

            return View(new ProductWithRelatedVM()
            {
                Products = products.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page,
                Query = productFilterVM.query,
                Categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken),
                Brands = await _brandRepository.GetAsync(cancellationToken: cancellationToken),
            });
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
        {
            //return View(new UpsertProductVM()
            //{
            //    NewCategories = _context.Categories.AsEnumerable().Select(e=>new SelectListItem()
            //    {
            //        Text = e.Name,
            //        Value = e.Id.ToString()
            //    }),
            //    NewBrands = _context.Brands.AsEnumerable().Select(e => new SelectListItem()
            //    {
            //        Text = e.Name,
            //        Value = e.Id.ToString()
            //    }),
            //});

            return View(new UpsertProductVM()
            {
                Categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken),
                Brands = await _brandRepository.GetAsync(cancellationToken: cancellationToken),
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile mainImg, List<IFormFile> subImgs, CancellationToken cancellationToken = default) 
        {
            if(mainImg is not null && mainImg.Length > 0)
            {
                // Save Main Img in wwwroot
                var fileName = _productService.SaveImg(mainImg);

                if(fileName is not null)
                {
                    // Save Main Img name in DB
                    product.MainImg = fileName;
                }
            }

            //_context.Products.Add(product);
            //_context.SaveChanges();
            await _productRepository.CreateAsync(product, cancellationToken);
            await _productRepository.CommitAsync(cancellationToken);

            if(subImgs.Any())
            {
                foreach (var item in subImgs)
                {
                    if (item is not null && item.Length > 0)
                    {
                        var fileName = _productService.SaveImg(item, ProductImgType.SubImg);

                        if (fileName is not null)
                        {
                            //_context.ProductSubImgs.Add(new()
                            //{
                            //    SubImg = fileName,
                            //    ProductId = product.Id
                            //});
                            await _productSubImgRepository.CreateAsync(new ProductSubImg()
                            {
                                SubImg = fileName,
                                ProductId = product.Id
                            }, cancellationToken: cancellationToken);
                        }
                    }
                }
            }
            await _productSubImgRepository.CommitAsync(cancellationToken);

            TempData["success_notification"] = "Add Product Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken = default)
        {
            //var product = _context.Products.SingleOrDefault(e => e.Id == id);
            var product = await _productRepository.GetOneAsync(e=>e.Id == id);

            if (product is null) return NotFound();

            return View(new UpsertProductVM()
            {
                Product = product,
                ProductSubImgs = await _productSubImgRepository.GetAsync(e => e.ProductId == id),
                Categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken),
                Brands = await _brandRepository.GetAsync(cancellationToken: cancellationToken),
            });
        }

        [HttpPost]
        public async Task<IActionResult> Update(Product product, IFormFile? mainImg, List<IFormFile>? subImgs, CancellationToken cancellationToken = default) // string name, string Description, bool status
        {
            //var productInDb = _context.Products.AsNoTracking().SingleOrDefault(e => e.Id == product.Id);
            var productInDb = await _productRepository.GetOneAsync(e => e.Id == product.Id, tracked: false);

            if (productInDb is null) return NotFound();

            if(mainImg is not null && mainImg.Length > 0)
            {
                // Save new Main Img in wwwroot
                var fileName = _productService.SaveImg(mainImg);

                // Remove old Main Img from wwwroot
                _productService.RemoveImg(productInDb.MainImg);

                // Save new Main Img in DB
                if (fileName is not null) product.MainImg = fileName;
            }
            else
                product.MainImg = productInDb.MainImg;

            //_context.Products.Update(product);
            //_context.SaveChanges();

            _productRepository.Update(product);
            await _productRepository.CommitAsync(cancellationToken);

            if(subImgs is not null && subImgs.Any())
            {
                var productSubImgs = await _productSubImgRepository.GetAsync(e => e.ProductId == product.Id);

                foreach (var item in productSubImgs)
                    _productService.RemoveImg(item.SubImg, ProductImgType.SubImg);

                _productSubImgRepository.DeleteRange(productSubImgs);

                foreach (var item in subImgs)
                {
                    if (item is not null && item.Length > 0)
                    {
                        var fileName = _productService.SaveImg(item, ProductImgType.SubImg);

                        if (fileName is not null)
                        {
                            await _productSubImgRepository.CreateAsync(new ProductSubImg()
                            {
                                SubImg = fileName,
                                ProductId = product.Id
                            }, cancellationToken: cancellationToken);
                        }
                    }
                }
                await _productSubImgRepository.CommitAsync(cancellationToken);
            }

            TempData["success_notification"] = "Update Product Successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            //var product = _context.Products.SingleOrDefault(e => e.Id == id);
            var product = await _productRepository.GetOneAsync(e => e.Id == id);

            if (product is null) return NotFound();

            var productSubImgs = await _productSubImgRepository.GetAsync(e => e.ProductId == product.Id);

            foreach (var item in productSubImgs)
                _productService.RemoveImg(item.SubImg, ProductImgType.SubImg);

            // Remove old Main Img from wwwroot
            _productService.RemoveImg(product.MainImg);

            _productRepository.Delete(product);
            await _productRepository.CommitAsync(cancellationToken);

            TempData["success_notification"] = "Remove Product Successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
