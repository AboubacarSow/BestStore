using BestStoreApp.Services.ApplicationDbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BestStoreApp.Models;

namespace BestStoreApp.Controllers;

[Authorize(Roles = "admin")]
[Route("/Admin/[controller]/{action=Index}/{id?}")]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly int PageSize = 5;
    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int pageNumber)
    {
        IQueryable<Order> query = _context.Orders;
        if (pageNumber <= 0)
        {
            pageNumber = 1;
        }


        decimal count = query.Count();
        int totalPages = (int)Math.Ceiling(count / PageSize);
        query = query.Skip((pageNumber - 1) * PageSize).Take(PageSize);
        var orders = query.ToList();

        ViewBag.Orders = orders;
        ViewBag.PageNumber = pageNumber;
        ViewBag.TotalPages = totalPages;
        return View(orders);
    }
    public IActionResult Details(int id)
    {
        var order=_context.Orders.Find(id);

        ViewBag.NumOrders = _context.Orders.Where(o => o.ClientId == order!.ClientId).Count();
        return View(order);
    }
    public IActionResult Edit(int id,string? payment_status, string? order_status)
    {
        var order = _context.Orders.Find(id);
        if(order is null)
        {
            return RedirectToAction("Index", "Home");
        }
        if (string.IsNullOrEmpty(payment_status) && string.IsNullOrEmpty(order_status))
            return RedirectToAction("Details", new { id });
        if (!string.IsNullOrEmpty(payment_status)) order.PaymentStatus = payment_status;
        if (!string.IsNullOrEmpty(order_status)) order.OrderStatus = order_status;
        _context.SaveChanges();
        return RedirectToAction("Details", new { id });
    }
}
