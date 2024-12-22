using System;
using IdentityApp.Data;
using IdentityApp.Models.Account;
using IdentityApp.Utilities.Email.Abstracts;
using IdentityApp.Utilities.Email.Concrets;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StylishApp.Data.Entities;

namespace IdentityApp.Controllers;

public class AccountController : Controller
{
    readonly SignInManager<AppUser> _signInManager;
    readonly IEmailSender _emailSender;
    readonly UserManager<AppUser> _userManager;

    readonly IdentityContext _context;

    public AccountController(SignInManager<AppUser> signInManager, IEmailSender emailSender, UserManager<AppUser> userManager, IdentityContext context)
    {
        _signInManager = signInManager;
        _emailSender = emailSender;
        _userManager = userManager;
        _context = context;
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

        var user = new AppUser
        {
            UserName = model.Email,
            Email = model.Email,
        };



        var result = _userManager.CreateAsync(user, model.Password).Result;

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }


        var token = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;
        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { email = user.Email, token }, Request.Scheme);
        _emailSender.SendEmail(new Message([user.Email], "Confirm your email", confirmationLink));

        _context.Baskets.Add(new Basket { UserId = user.Id });
        _context.SaveChanges();

        return RedirectToAction("Login");
    }

    public IActionResult ConfirmEmail(string email, string token)
    {
        if (email == null || token == null)
            return RedirectToAction("Index", "Home");

        var user = _userManager.FindByEmailAsync(email).Result;

        if (user == null) return NotFound();

        var result = _userManager.ConfirmEmailAsync(user, token).Result;

        if (!result.Succeeded) return NotFound();

        return RedirectToAction("Login");
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ForgotPassword(string email)
    {
        if (email == null)
        {
            ViewData["ErrorMessage"] = "Email is required.";
            return View();
        }

        var user = _userManager.FindByEmailAsync(email).Result;

        if (user == null)
        {
            ViewData["ErrorMessage"] = "User not found.";
            return View();
        }

        var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;
        var confirmationLink = Url.Action("ResetPassword", "Account", new { email = user.Email, token }, Request.Scheme);
        _emailSender.SendEmail(new Message([user.Email], "ResetPassword", confirmationLink));

        return View("EmailSend");
    }

    public IActionResult ResetPassword()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ResetPassword(ResetPasswordVM model)
    {
        if (!ModelState.IsValid)
            return View();

        var user = _userManager.FindByEmailAsync(model.Email).Result;

        if (user == null)
        {
            ModelState.AddModelError("email", "User not found.");
            return View();
        }

        var result = _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword).Result;

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
