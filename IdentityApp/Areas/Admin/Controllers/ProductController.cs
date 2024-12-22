using IdentityApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StylishApp.Areas.Admin.Models.Product;
using StylishApp.Data.Entities;
using StylishApp.Utilities.FileService;

namespace StylishApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        readonly IdentityContext _context;
        readonly IFileService _fileService;

        public ProductController(IdentityContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public IActionResult Index()
        {
            List<Product> products = _context.Products.Include(p => p.Colors).ToList();

            ProductVM model = new ProductVM
            {
                Products = products
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            CreateProductVM model = new CreateProductVM()
            {
                AvailableColors = _context.Colors.ToList(),
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateProductVM model)
        {
            if (!ModelState.IsValid)
                return View();

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                Colors = new List<Color>()
            };

            if (model.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Please select an image file");
                return View();
            }

            if (!_fileService.IsImage(model.ImageFile.ContentType))
            {
                ModelState.AddModelError("ImageFile", "Please select an image file");
                return View();
            }

            if (!_fileService.IsAvailableSize(model.ImageFile.Length, 500))
            {
                ModelState.AddModelError("ImageFile", "Please select an image file less than 100KB");
                return View();
            }

            product.ImageName = _fileService.Upload(model.ImageFile, "upload/product");


            List<Color> colors = _context.Colors.Where(c => model.ColorIds.Contains(c.Id)).ToList();

            product.Colors = colors;

            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            Product? product = _context.Products.Include(p => p.Colors).FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            UpdateProductVM model = new UpdateProductVM
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageName = product.ImageName,
                Stock = product.Stock,
                ColorIds = product.Colors.Select(c => c.Id).ToList(),
                AvailableColors = _context.Colors.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Update(int id, UpdateProductVM model)
        {
            if (!ModelState.IsValid)
                return View();

            Product? product = _context.Products.Include(p => p.Colors).FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Stock = model.Stock;

            if (model.ImageFile != null)
            {
                if (!_fileService.IsImage(model.ImageFile.ContentType))
                {
                    ModelState.AddModelError("ImageFile", "Please select an image file");
                    return View();
                }

                if (!_fileService.IsAvailableSize(model.ImageFile.Length, 500))
                {
                    ModelState.AddModelError("ImageFile", "Please select an image file less than 100KB");
                    return View();
                }

                _fileService.Delete(product.ImageName, "upload/product");

                product.ImageName = _fileService.Upload(model.ImageFile, "upload/product");
            }

            List<Color> colors = _context.Colors.Where(s => model.ColorIds.Contains(s.Id)).ToList();

            product.Colors = colors;

            // _context.Products.Update(product);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Product? product = _context.Products.Find(id);

            if (product == null)
                return NotFound();

            _fileService.Delete(product.ImageName, "upload/product");

            _context.Products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
