using BestStoreApp.Infrastructure.Utilities;
using BestStoreApp.Models;
using BestStoreApp.Services.ApplicationDbContext;
using BestStoreApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Microsoft.AspNetCore.Authorization;

namespace BestStoreApp.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly StripeSettings Stripe;
        private readonly decimal shippingFee;
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<PaymentController> logger;
        public PaymentController(IOptions<StripeSettings> stripeSettings, IConfiguration configuration,
            ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<PaymentController> logger)
        {
            Stripe = stripeSettings.Value;
            this.shippingFee = configuration.GetValue<decimal>("CartSettings:ShippingFee");
            this.context = context;
            this.userManager = userManager;
            this.logger = logger;
        }
        public IActionResult Ckeckout()
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, context);
            decimal total = CartHelper.GetSubtotal(cartItems) + shippingFee;
            string deliveryAddress = TempData["DeliveryAddress"] as string ?? "";
            TempData.Keep();
            ViewBag.StripePublishableKey = Stripe.PublishableKey;
            ViewBag.DeliveryAddress = deliveryAddress;
            ViewBag.AmountInCents = (long)(total * 100);
            ViewBag.DisplayAmount = total.ToString("0.00");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestDto request)
        {
            try
            {
                var options = new ChargeCreateOptions
                {
                    Amount =request.Amount, // Amount in cents (e.g., 10.00 EUR)
                    Currency = "USD",
                    Source = request.Token,
                    Description = "Test Payment"
                };

                var service = new ChargeService();
                var charge = await service.CreateAsync(options);
                
                TempData["PaymentId"] = charge.Id;
                logger.LogInformation(message: $"Stripe Payment succed.ID:{charge.Id}");
                return Json(new { success = true, redirectUrl = Url.Action("Success") });
            }
            catch (StripeException e)
            {
                logger.LogError(e, "Stripe payment failed");
                return Json(new
                {
                    success = false,
                    error = "Payment failed. Please try again." // Generic user message
                });
            }
        }
        public IActionResult Success()
        {
            return View();
        }
    }
}
