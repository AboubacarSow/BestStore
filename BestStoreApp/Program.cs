using BestStoreApp.Config;
using BestStoreApp.Infrastructure.Utilities;
using BestStoreApp.Models;
using BestStoreApp.Services;
using BestStoreApp.Services.ApplicationDbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
//Configuring Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

}).AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection(nameof(StripeSettings)));
StripeConfiguration.ApiKey = builder.Configuration.GetValue<string>("StripeSettings:SecretKey");


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// create the roles and the first admin user if not available yet
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>))
        as UserManager<ApplicationUser>;
    var roleManager = scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>))
        as RoleManager<IdentityRole>;

    await IdentityConfig.SeedDataAsync(userManager, roleManager);
}
app.Run();
