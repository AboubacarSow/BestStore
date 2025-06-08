using BestStoreApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BestStoreApp.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    public IActionResult Register()
    {
        if (signInManager.IsSignedIn(User))
            return RedirectToAction(nameof(Index), nameof(HomeController));
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        if (signInManager.IsSignedIn(User))
            return RedirectToAction(nameof(Index), nameof(HomeController));
        if (!ModelState.IsValid)
        {
            return View(registerDto);
        }
        var user = new ApplicationUser
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            UserName = registerDto.Email, // UserName will be used to authenticate the user
            Email = registerDto.Email,
            PhoneNumber = registerDto.PhoneNumber,
            Address = registerDto.Address,
            CreatedAt = DateTime.Now,
        };
        var result=await userManager.CreateAsync(user, registerDto.Password);
        if(result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "client");
            await signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index","Home");
        }
        foreach(var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        return View(registerDto);
    }
}
