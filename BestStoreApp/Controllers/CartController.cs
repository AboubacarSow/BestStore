using BestStoreApp.Models;
using BestStoreApp.Services.ApplicationDbContext;
using BestStoreApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BestStoreApp.Controllers;


public class CartController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly decimal? shippingFee;
    private readonly ApplicationDbContext context;
    public CartController(ApplicationDbContext context, IConfiguration configuration
             , UserManager<ApplicationUser> userManager)
    {
        this.context = context;
        this.userManager = userManager;
        shippingFee = configuration.GetValue<decimal>("CartSettings:ShippingFee");
    }

    public IActionResult Index()
    {
        List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, context);
        decimal subtotal = CartHelper.GetSubtotal(cartItems);


        ViewBag.CartItems = cartItems;
        ViewBag.ShippingFee = shippingFee;
        ViewBag.Subtotal = subtotal;
        ViewBag.Total = subtotal + shippingFee;

        return View();
    }
    [Authorize]
    [HttpPost]
    public IActionResult Index(CheckOutDto checkDto)
    {
        List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, context);
        decimal subtotal = CartHelper.GetSubtotal(cartItems);


        ViewBag.CartItems = cartItems;
        ViewBag.ShippingFee = shippingFee;
        ViewBag.Subtotal = subtotal;
        ViewBag.Total = subtotal + shippingFee;
        if (!ModelState.IsValid) return View(checkDto);

        if (cartItems.Count <= 0)
        {
            ViewBag.ErrorMessage = "Your cart is empty";
            return View(checkDto);
        }

        TempData["DeliveryAddress"] = checkDto.DeliveryAddress;
        TempData["PaymentMethod"] = checkDto.PaymentMethod;
        if (checkDto.PaymentMethod == "stripe" || checkDto.PaymentMethod == "credit_card")
        {
            return RedirectToAction("Checkout", "Payment");
        }
        return RedirectToAction("Confirm");
    }
    [Authorize]
    public IActionResult Confirm()
    {
        List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, context);
        decimal total =(decimal)( CartHelper.GetSubtotal(cartItems) + shippingFee)!;
        int cartSize = 0;
        foreach (var item in cartItems)
        {
            cartSize += item.Quantity;
        }


        string deliveryAddress = TempData["DeliveryAddress"] as string ?? "";
        string paymentMethod = TempData["PaymentMethod"] as string ?? "";
        TempData.Keep();


        if (cartSize == 0 || deliveryAddress.Length == 0 || paymentMethod.Length == 0)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewBag.DeliveryAddress = deliveryAddress;
        ViewBag.PaymentMethod = paymentMethod;
        ViewBag.Total = total;
        ViewBag.CartSize = cartSize;

        return View();
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Confirm(int any)
    {
        var cartItems = CartHelper.GetCartItems(Request, Response, context);

        string deliveryAddress = TempData["DeliveryAddress"] as string ?? "";
        string paymentMethod = TempData["PaymentMethod"] as string ?? "";
        TempData.Keep();

        if (cartItems.Count == 0 || deliveryAddress.Length == 0 || paymentMethod.Length == 0)
        {
            return RedirectToAction("Index", "Home");
        }

        var appUser = await userManager.GetUserAsync(User);
        if (appUser == null)
        {
            return RedirectToAction("Index", "Home");
        }
        var order = new Order
        {
            ClientId = appUser.Id,
            Items = cartItems,
            ShippingFee = (decimal)shippingFee!,
            DeliveryAddress = deliveryAddress,
            PaymentMethod = paymentMethod,
            PaymentStatus = "pending",
            PaymentDetails = "",
            OrderStatus = "created",
            CreatedAt = DateTime.Now,
        };
        context.Orders.Add(order);
        context.SaveChanges();
        //delete shopping cookie
        Response.Cookies.Delete("shopping_cart");

        ViewBag.SuccessMessage = "Order created successfully";
        return View();
    }
}
