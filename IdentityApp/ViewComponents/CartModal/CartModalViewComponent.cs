using System;
using IdentityApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StylishApp.Data.Entities;
using StylishApp.Models.Cart;


namespace StylishApp.ViewComponents.CartModal;

public class CartModalViewComponent : ViewComponent
{
    readonly UserManager<AppUser> _userManager;
    readonly IdentityContext _context;

    public CartModalViewComponent(UserManager<AppUser> userManager, IdentityContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    public IViewComponentResult Invoke()
    {
        var model = new CartVM();

        var user = _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User).Result;

        if (user == null)
        {
            return Content("Unauthorized");
        }

        var basket = _context.Baskets.Include(b => b.Items).ThenInclude(bi => bi.Product).FirstOrDefault(x => x.UserId == user.Id);

        if (basket == null)
        {
            return View(model);
        }

        model.Items = basket.Items.ToList();

        return View(model);
    }
}
