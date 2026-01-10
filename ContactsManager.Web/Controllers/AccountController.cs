using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTO.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Web.Controllers;

[Route("[Controller]/[action]")]
[AllowAnonymous]
public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromForm] Register register)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Errors = ModelState.Values.SelectMany(entry => entry.Errors.Select(error => error.ErrorMessage));
            return View(register);
        }

        User user = new User()
        {
            Name = register.Name,
            UserName = register.Email,
            PhoneNumber = register.Phone,
            Email = register.Email
        };

        IdentityResult identityResult = await _userManager.CreateAsync(user, register.Password);
        if (identityResult.Succeeded)
        {
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        foreach (IdentityError error in identityResult.Errors)
        {
            ModelState.AddModelError("Register", error.Description);
        }
        return View(register);
    }

    [HttpGet]
    public IActionResult SignIn()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignIn(Login login)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Errors = ModelState.Values.SelectMany(entry => entry.Errors).Select(error => error.ErrorMessage);
            return View(login);
        }
        
        var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);

        if (result is null || !result.Succeeded)
        {
            ModelState.AddModelError("SignIn", "Sorry Invalid Email Or Password");
            return View(login);
        }
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

}
