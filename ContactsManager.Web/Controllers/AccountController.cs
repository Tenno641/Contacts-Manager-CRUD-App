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
    private readonly RoleManager<Role> _roleManager;
    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
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
            if (register.UserType == UserType.Admin)
            {
                await CreateRole(UserType.Admin);
                await _userManager.AddToRoleAsync(user, UserType.Admin.ToString());
            }
            else
            {
                await CreateRole(UserType.User);
                await _userManager.AddToRoleAsync(user, UserType.User.ToString());
            }
            return RedirectToAction(actionName: nameof(HomeController.Index), "Home");
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
    public async Task<IActionResult> SignIn(Login login, string returnUrl)
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
        return LocalRedirect(returnUrl);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
    {
        User? user = await _userManager.FindByEmailAsync(email);
        if (user is null) return Json(true);
        return Json(false);
    }
    private async Task CreateRole(UserType userType)
    {
        if (!await _roleManager.RoleExistsAsync(userType.ToString()))
        {
            Role role = new Role() { Name = userType.ToString() };
            await _roleManager.CreateAsync(role);
        }
    }
}
