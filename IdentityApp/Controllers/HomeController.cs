using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Data.Entities;

namespace IdentityApp.Controllers;

public class HomeController : Controller
{
    readonly UserManager<AppUser> _userManager;

    public HomeController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Subscribe(string email)
    {
        var existing = _userManager.FindByEmailAsync(email).Result;

        if (existing == null)
        {
            ViewData["ErrorMessage"] = "User not found";
            return View("Index");
        }

        if (!existing.SubscribeNewsletter)
        {
            existing.SubscribeNewsletter = true;
            var result = _userManager.UpdateAsync(existing).Result;

            if (!result.Succeeded)
            {
                ViewData["ErrorMessage"] = "Error updating user";
                return View("Index");
            }
        }
        
        return RedirectToAction("Index");
    }
}
