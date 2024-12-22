using IdentityApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StylishApp.Data.Entities;
using StylishApp.Models;
using StylishApp.Models.Cart;

namespace StylishApp.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CartController : Controller
    {
        readonly UserManager<AppUser> _userManager;
        readonly IdentityContext _context;

        readonly StripeSettings _stripeSettings;

        public CartController(UserManager<AppUser> userManager, IdentityContext context, IOptions<StripeSettings> stripeSettings)
        {
            _userManager = userManager;
            _context = context;
            _stripeSettings = stripeSettings.Value;
        }

        public ActionResult Index()
        {
            ViewBag.StripePublishableKey = _stripeSettings.PublishableKey;
            var model = new CartVM();

            var user = _userManager.GetUserAsync(User).Result;

            if (user == null)
            {
                return Unauthorized();
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
}
