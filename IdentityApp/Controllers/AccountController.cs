using System;
using IdentityApp.Models.Account;
using IdentityApp.Utilities.Email.Abstracts;
using IdentityApp.Utilities.Email.Concrets;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers;

public class AccountController : Controller
{
    readonly SignInManager<IdentityUser> _signInManager;
    readonly IEmailSender _emailSender;
    readonly UserManager<IdentityUser> _userManager;

    public AccountController(SignInManager<IdentityUser> signInManager, IEmailSender emailSender, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _emailSender = emailSender;
        _userManager = userManager;
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
