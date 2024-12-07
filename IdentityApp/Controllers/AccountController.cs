using System;
using IdentityApp.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers;

public class AccountController : Controller
{
    readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(SignupVM model)
    {
        if (!ModelState.IsValid)
            return View();

        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        var result = _signInManager.UserManager.CreateAsync(user, model.Password).Result;

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }

        return RedirectToAction("Login");
    }
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginVM model)
    {
        if (!ModelState.IsValid)
            return View();

        var result = _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false).Result;

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        _signInManager.SignOutAsync().Wait();
        return RedirectToAction("Login");
    }
}
