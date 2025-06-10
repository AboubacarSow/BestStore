using BestStoreApp.Infrastructure.Utilities;
using BestStoreApp.Models;
using BestStoreApp.Services.ApplicationDbContext;
using BestStoreApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Microsoft.AspNetCore.Authorization;
using Stripe.V2;
using Microsoft.AspNetCore.Identity.UI.Services;

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
        private readonly IEmailSender emailSender;
        public PaymentController(IOptions<StripeSettings> stripeSettings, IConfiguration configuration,
            ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<PaymentController> logger, IEmailSender emailSender)
        {
            Stripe = stripeSettings.Value;
            this.shippingFee = configuration.GetValue<decimal>("CartSettings:ShippingFee");
            this.context = context;
            this.userManager = userManager;
            this.logger = logger;
            this.emailSender = emailSender;
        }
        public IActionResult Checkout()
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
                    Amount = request.Amount, // Amount in cents (e.g., 10.00 EUR)
                    Currency = "USD",
                    Source = request.Token,
                    Description = "Test Payment"
                };

                var service = new ChargeService();
                var charge = await service.CreateAsync(options);

                TempData["PaymentId"] = charge.Id;

                logger.LogInformation(message: $"Stripe Payment succed.ID:{charge.Id}");
                var details = new PaymentDetails
                {
                    Id=charge.PaymentIntentId,
                    StripeChargeId = charge.Id,
                    Amount = charge.Amount,
                    Currency = charge.Currency,
                    Method = charge.PaymentMethod,
                    PaidAt = charge.Created,
                    Status = charge.Status,
                    CardLast4Digits = charge.PaymentMethodDetails.Card?.Last4!,
                   
                };
                var appUser = await userManager.GetUserAsync(User);
                if (appUser == null) return RedirectToAction("Index", "Home");
                var body = $"<h3>Thank you for your purchase, {appUser?.FirstName} {appUser?.LastName}!</h3>" +
                        $"<p>Amount: <strong>{charge.Amount:C}</strong></p>" +
                        $"<p>Transaction ID: {charge.Id}</p>" +
                        $"<p>View your receipt in Stripe:" +
                        $"<a href='https://dashboard.stripe.com/payments/{charge.Id}'> Receipt</a> </p>";
                var subject = "Your Receipt for your purchase";
                await emailSender.SendEmailAsync(appUser?.Email!,subject,body);
                await SaveOrderAsync(request.DeliveredAddress, details);
                return Json(new { success = true, redirectUrl = Url.Action("Success","Payment") });
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


        private async Task SaveOrderAsync(string address, PaymentDetails paymentDetails)
        {
            var cartItems = CartHelper.GetCartItems(Request, Response, context);

            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return;
            }

            // save the order
            var order = new Order
            {
                ClientId = appUser.Id,
                Items = cartItems,
                ShippingFee = shippingFee,
                DeliveryAddress = address,
                PaymentMethod = "stripe",
                PaymentStatus = "completed",
                PaymentDetailsId=paymentDetails.Id,
                OrderStatus = "on processing",
                CreatedAt = DateTime.Now,
            };

            context.Orders.Add(order);
            context.SaveChanges();


            // delete the shopping cart cookie
            Response.Cookies.Delete("shopping_cart");
        }
    }
}
