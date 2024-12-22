using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using StylishApp.Data.Entities;
using IdentityApp.Data;
using StylishApp.Models.Home;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Controllers;

public class HomeController : Controller
{
    readonly UserManager<AppUser> _userManager;
    readonly IdentityContext _context;

    public HomeController(UserManager<AppUser> userManager, IdentityContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public IActionResult Index()
    {
        var model = new HomeVM
        {
            Products = _context.Products.ToList()
        };

        return View(model);
    }

    public IActionResult GetProductDetails(int productId)
    {
        // Veritabanından ürün bilgisini al
        var product = _context.Products.Include(p => p.Colors).FirstOrDefault(p => p.Id == productId);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        // Partial view ile modal içeriğini döndür
        return PartialView("_ProductDetailsModal", product);
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
