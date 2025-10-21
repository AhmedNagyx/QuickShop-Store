using Microsoft.AspNetCore.Mvc;
using QuickShop.Models;
using QuickShop.Services;

namespace QuickShop.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly int pageSize = 8;


        public StoreController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index(int pageIndex, string? search, string? author, string? category, string? sort)
        {
            IQueryable<Product> query = context.Products;

            //search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search));
            }

            //filter by author
            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(p => p.Author == author);
            }
            //filter by category
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }
            //sort
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "price_asc":
                        query = query.OrderBy(p => p.Price);
                        break;
                    case "price_desc":
                        query = query.OrderByDescending(p => p.Price);
                        break;
                    case "oldest":
                        query = query.OrderBy(p => p.Id);
                        break;
                    default:
                        query = query.OrderByDescending(p => p.Id);
                        break;
                }
            }

               

                //pagination
                if (pageIndex < 1)
                {
                    pageIndex = 1;
                }
                decimal count = query.Count();
                int totalPages = (int)Math.Ceiling(count / pageSize);
                query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

                var products = query.ToList();

                ViewBag.Products = products;
                ViewBag.PageIndex = pageIndex;
                ViewBag.TotalPages = totalPages;

                var storeSearchModel = new StoreSeachModel
                {
                    Search = search,
                    Author = author,
                    Category = category,
                    Sort = sort
                };

                return View(storeSearchModel);
            }
        }
    }

