using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                Categories = _context.Categories.AsEnumerable(),
                Brands = _context.Brands.AsEnumerable(),
            });
        }

        [HttpGet]
        public IActionResult Create()
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
                Categories = _context.Categories.AsEnumerable(),
                Brands = _context.Brands.AsEnumerable()
            });
        }

        [HttpPost]
        public IActionResult Create(Product product, IFormFile mainImg, List<IFormFile> subImgs) 
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

            _context.Products.Add(product);
            _context.SaveChanges();

            if(subImgs.Any())
            {
                foreach (var item in subImgs)
                {
                    if (item is not null && item.Length > 0)
                    {
                        var fileName = _productService.SaveImg(item, ProductImgType.SubImg);

                        if (fileName is not null)
                        {
                            _context.ProductSubImgs.Add(new()
                            {
                                SubImg = fileName,
                                ProductId = product.Id
                            });
                        }
                    }
                }
            }
            _context.SaveChanges();

            TempData["success_notification"] = "Add Product Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var product = _context.Products.SingleOrDefault(e => e.Id == id);

            if (product is null) return NotFound();

            return View(new UpsertProductVM()
            {
                Product = product,
                ProductSubImgs = _context.ProductSubImgs.Where(e => e.ProductId == id),
                Categories = _context.Categories.AsEnumerable(),
                Brands = _context.Brands.AsEnumerable(),
            });
        }

        [HttpPost]
        public IActionResult Update(Product product, IFormFile? mainImg, List<IFormFile>? subImgs) // string name, string Description, bool status
        {
            var productInDb = _context.Products.AsNoTracking().SingleOrDefault(e => e.Id == product.Id);

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

            _context.Products.Update(product);
            _context.SaveChanges();

            if(subImgs is not null && subImgs.Any())
            {
                var productSubImgs = _context.ProductSubImgs.Where(e => e.ProductId == product.Id);

                foreach (var item in productSubImgs)
                    _productService.RemoveImg(item.SubImg, ProductImgType.SubImg);

                _context.ProductSubImgs.RemoveRange(productSubImgs);

                foreach (var item in subImgs)
                {
                    if (item is not null && item.Length > 0)
                    {
                        var fileName = _productService.SaveImg(item, ProductImgType.SubImg);

                        if (fileName is not null)
                        {
                            _context.ProductSubImgs.Add(new()
                            {
                                SubImg = fileName,
                                ProductId = product.Id
                            });
                        }
                    }
                }
                _context.SaveChanges();
            }

            TempData["success_notification"] = "Update Product Successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var product = _context.Products.SingleOrDefault(e => e.Id == id);
            
            if (product is null) return NotFound();

            var productSubImgs = _context.ProductSubImgs.Where(e => e.ProductId == product.Id);

            foreach (var item in productSubImgs)
                _productService.RemoveImg(item.SubImg, ProductImgType.SubImg);

            // Remove old Main Img from wwwroot
            _productService.RemoveImg(product.MainImg);

            _context.Products.Remove(product);
            _context.SaveChanges();

            TempData["success_notification"] = "Remove Product Successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
