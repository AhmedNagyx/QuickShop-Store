using Microsoft.AspNetCore.Mvc;
using QuickShop.Models;
using QuickShop.Services;

namespace QuickShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;
        private readonly int pageSize = 10;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index(int pageIndex)
        {
            IQueryable<Product> query = context.Products.OrderByDescending(x => x.Id);
            //pagination
            if(pageIndex < 1)
            {
                pageIndex = 1;
            }
            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            var products = query.ToList();

            ViewData["PageIndex"] = pageIndex;
            ViewData["TotalPages"] = totalPages;
            return View(products);
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Product image is required.");
            }
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            //save image
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

            string imagePath = environment.WebRootPath + "/Images/" + newFileName;
            using (var stream = System.IO.File.Create(imagePath))
            {
                productDto.ImageFile!.CopyTo(stream);
            }

            //save product
            Product product = new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                Description = productDto.Description,
                Brand = productDto.Brand,
                Category = productDto.Category,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now
            };
            context.Products.Add(product);
            context.SaveChanges();

            return RedirectToAction("Index", "Products");
        }

        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            // create ProductDto from Product
            var productDto = new ProductDto
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description
            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt;
            return View(productDto);
        }
        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }
            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt;
                return View(productDto);
            }
            //update image if new image is uploaded
            string newFileName = product.ImageFileName;
            if (productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFile.FileName);
                string imagePath = environment.WebRootPath + "/Images/" + newFileName;
                using (var stream = System.IO.File.Create(imagePath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }
                //delete old image
                string oldImagePath = environment.WebRootPath + "/Images/" + product.ImageFileName;
                System.IO.File.Delete(oldImagePath);
            }
            //update product
            product.Name = productDto.Name;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.ImageFileName = newFileName;
            context.SaveChanges();

            return RedirectToAction("Index", "Products");
        }

        public IActionResult Delete(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }
            //delete image
            string imagePath = environment.WebRootPath + "/Images/" + product.ImageFileName;
            System.IO.File.Delete(imagePath);
            //delete product
            context.Products.Remove(product);
            context.SaveChanges(true);

            return RedirectToAction("Index", "Products");
        }
    }
}
