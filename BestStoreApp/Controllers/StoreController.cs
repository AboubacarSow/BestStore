using BestStoreApp.Models;
using BestStoreApp.Services.ApplicationDbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BestStoreApp.Controllers;

public class StoreController(ApplicationDbContext context) : Controller
{
    private readonly int PageSize = 8;
    public IActionResult Index(int pageNumber,string? search,string? category,string? brand,string? sort)
    {
        IQueryable<Product> query= context.Products;
        // search functionality
        if (search != null && search.Length > 0)
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        // filter functionality
        if (brand != null && brand.Length > 0)
        {
            query = query.Where(p => p.Brand.Contains(brand));
        }

        if (category != null && category.Length > 0)
        {
            query = query.Where(p => p.Category.Name.Contains(category));
        }

        // sort functionality
        if (sort == "price_asc")
        {
            query = query.OrderBy(p => p.Price);
        }
        else if (sort == "price_desc")
        {
            query = query.OrderByDescending(p => p.Price);
        }
        else
        {
            // newest products first
            query = query.OrderByDescending(p => p.Id);
        }

        decimal count = query.Count();
        int totalPage = (int)Math.Ceiling(count / PageSize);
        if (pageNumber < 1)
            pageNumber = 1;
        query = query.Skip((pageNumber - 1) * PageSize).Take(PageSize);
        var products = query.ToList();
        ViewData["TotalPages"] = totalPage;
        ViewData["PageNumber"] = pageNumber;
        ViewBag.Products = products;
        GetCategories();
        GetBrands();
        var requestParameters = new RequestParameters
        {
            Search=search!,
            Category=category!,
            Sort=sort!,
            Brand=brand!

        };
        return View(requestParameters);
    }
    private void GetCategories()
    {
        ViewBag.Categories = (from c in context.Categories
                              select new SelectListItem
                              {
                                  Text = c.Name,
                                  Value = c.Name.ToString()
                              }).ToList();
    } 
    private void GetBrands()
    {
        ViewBag.Brands = (from c in context.Products
                              select new SelectListItem
                              {
                                  Text = c.Brand,
                                  Value = c.Brand.ToString()
                              }).ToList();
    }
    [HttpGet]
    public IActionResult Details(int id)
    {
        var product = context.Products.Find(id);
        if(product==null)
            return RedirectToAction(nameof(Index));
        return View(product);
    }
}
