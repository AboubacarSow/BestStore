using BestStoreApp.Infrastructure.Utilities;
using BestStoreApp.Models;
using BestStoreApp.Services.ApplicationDbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BestStoreApp.Controllers;
[Authorize(Roles ="Admin")]
[Route("/Admin/[controller]/{action=Index}/{id?}")]
public class ProductsController(ApplicationDbContext context) : Controller
{
    private int PageSize { get; set; } = 5;
    public IActionResult Index(int pageNumber, string? search, string? column,string? orderBy)
    {
        IQueryable<Product> query = context.Products;
        query = query.OrderByDescending(p => p.Id);
        if(search!=null)
            query = query.Where(c => c.Name.Contains(search) || c.Brand.Contains(search));
        if (column == "Name")
        {
            if (orderBy == "desc")
                query = query.OrderByDescending(c => c.Name);
            else
                query = query.OrderBy(c => c.Name); 
        }
        else if (column == "Brand")
        {
            if (orderBy == "desc")
                query = query.OrderByDescending(c => c.Brand);
            else
                query = query.OrderBy(c => c.Brand); 
        }
        else if (column == "CategoryId")
        {
            if (orderBy == "desc")
                query = query.OrderByDescending(c => c.CategoryId);
            else
                query = query.OrderBy(c => c.CategoryId); 
        }
        else if (column == "Price")
        {
            if (orderBy == "desc")
                query = query.OrderByDescending(c => c.Price);
            else
                query = query.OrderBy(c => c.Price);
        }
        else if (column == "CreateAt")
        {
            if (orderBy == "desc")
                query = query.OrderByDescending(c => c.CreateAt);
            else
                query = query.OrderBy(c => c.CreateAt);
        }
        else
        {
            if (orderBy == "desc") query=query.OrderByDescending(c => c.Id);
            else query= query.OrderBy(p => p.Id);
        }

        decimal count=query.Count();
        int totalPage = (int)Math.Ceiling(count / PageSize);
        if(pageNumber < 1)
            pageNumber=1;
        query=query.Skip((pageNumber-1)*PageSize).Take(PageSize);
        var products = query.ToList();
        ViewData["TotalPages"] = totalPage;
        ViewData["PageNumber"] = pageNumber;
        ViewData["search"] = search ?? "";
        ViewData["Column"] = column ?? "";
        ViewData["OrderBy"] = orderBy ?? "";

        return View(products);
    }
    public IActionResult Create()
    {
        GetCategories();
        return View();
    }
    private void GetCategories()
    {
        ViewBag.Categories = (from c in context.Categories
                              select new SelectListItem
                              {
                                 Text = c.Name,
                                 Value=c.Id.ToString()
                              }).ToList();
    }
    [HttpPost]
    public IActionResult Create([FromForm]ProductDto product)
    {
        if (!ModelState.IsValid)
        {
            GetCategories();
            return View(product);
        }
        var imageUrl = MediaService.UploadImage(product.ImageFile!);
        var new_product = new Product
        {
            Name = product.Name,
            CategoryId = product.CategoryId,
            Category = context.Categories.FirstOrDefault(c=>c.Id==product.CategoryId)!,
            ImageUrl = imageUrl,
            Brand = product.Brand,
            Description = product.Description,
            Price = product.Price,
            CreateAt = DateTime.UtcNow
        };
        context.Products.Add(new_product);
        context.SaveChanges();  
        return RedirectToAction("Index");   
       
            
    }
    [HttpGet]
    public IActionResult Edit(int  id)
    {
        var product = context.Products.FirstOrDefault(c => c.Id == id);
        if (product == null)
            return RedirectToAction(nameof(Index));
        var productDto = new EditProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Brand = product.Brand,
            CategoryId = product.CategoryId,
            Price = product.Price,
            Description = product.Description,
        };
        ViewData["ProductId"] = product.Id;
        ViewData["ImageUrl"] = product.ImageUrl;
        ViewData["CreatedAt"] = product.CreateAt.ToString("MM/dd/yyyy");
        GetCategories();
        return View(productDto);
    }
    [HttpPost]
    public IActionResult Edit(EditProductDto productDto)
    {
        var product = context.Products.Find(productDto.Id);

        if (product == null)
        {
            return RedirectToAction("Index", "Products");
        }


        if (!ModelState.IsValid)
        {
            ViewData["ProductId"] = product.Id;
            ViewData["ImageUrl"] = product.ImageUrl;
            ViewData["CreatedAt"] = product.CreateAt.ToString("MM/dd/yyyy");
            GetCategories();

            return View(productDto);
        }
        if (productDto.ImageFile != null)
        {
            string oldImageFullPath =Directory.GetCurrentDirectory() + "/wwwroot/products/" + product.ImageUrl;
            var imageUrl = MediaService.UploadImage(productDto.ImageFile);
            product.ImageUrl = imageUrl;
            System.IO.File.Delete(oldImageFullPath);
        }
        product.Name = productDto.Name;
        product.Brand = productDto.Brand;
        product.CategoryId = productDto.CategoryId;
        product.Category = context.Categories.Find(product.CategoryId)!;
        product.Description = productDto.Description;
        product.Price = productDto.Price;

        context.SaveChanges();
        return RedirectToAction(nameof(Index));

    }
    public IActionResult Delete([FromRoute]int id)
    {
        var product = context.Products.Find(id);
        if (product == null)
            return RedirectToAction(nameof(Index));
        context.Products.Remove(product);
        context.SaveChanges();
        string oldImageFullPath = Directory.GetCurrentDirectory() + "/wwwroot/products/" + product.ImageUrl;
        System.IO.File.Delete(oldImageFullPath);
        return RedirectToAction(nameof(Index));
    }

}
