using IdentityApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StylishApp.Data.Entities;

namespace StylishApp.Controllers
{
    public class BasketController : Controller
    {
        readonly IdentityContext _context;
        readonly UserManager<AppUser> _userManager;
        public BasketController(IdentityContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public IActionResult AddToBasket(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;

            if (user == null)
            {
                return Unauthorized();
            }

            var basket = _context.Baskets.Include(b => b.Items).FirstOrDefault(x => x.UserId == user.Id);

            if (basket == null)
            {
                basket = new Basket
                {
                    UserId = user.Id,
                };

                _context.Baskets.Add(basket);
                _context.SaveChanges();
            }

            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }


            if (product.Id == basket.Items.FirstOrDefault(x => x.ProductId == product.Id)?.ProductId)
            {
                var basketItemFinded = basket.Items.FirstOrDefault(x => x.ProductId == product.Id);

                if (product.Stock < basketItemFinded.Quantity + 1)
                {
                    return BadRequest("Stock is not enough");
                }

                basketItemFinded.Quantity++;
                _context.SaveChanges();
                return Ok("Successfully added to basket");
            }


            var basketItem = new BasketItem
            {
                BasketId = user.Basket.Id,
                ProductId = product.Id,
                Quantity = 1,
                Price = product.Price,
            };

            _context.BasketItems.Add(basketItem);
            _context.SaveChanges();

            return Ok("Successfully added to basket");
        }

        [HttpPost]
        public IActionResult RemoveFromBasket(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;

            if (user == null)
            {
                return Unauthorized();
            }

            var basket = _context.Baskets.Include(b => b.Items).FirstOrDefault(x => x.UserId == user.Id);

            if (basket == null)
            {
                return NotFound();
            }

            var basketItem = basket.Items.FirstOrDefault(x => x.Id == id);

            if (basketItem == null)
            {
                return NotFound();
            }

            _context.BasketItems.Remove(basketItem);
            _context.SaveChanges();

            return Ok("Successfully removed from basket");
        }


        [HttpPost]
        public IActionResult IncrementItem(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;

            if (user == null)
            {
                return Unauthorized();
            }

            var basket = _context.Baskets.Include(b => b.Items).FirstOrDefault(x => x.UserId == user.Id);

            if (basket == null)
            {
                return NotFound();
            }

            var basketItem = basket.Items.FirstOrDefault(x => x.Id == id);

            if (basketItem == null)
            {
                return NotFound();
            }

            var product = _context.Products.Find(basketItem.ProductId);

            if (product.Stock < basketItem.Quantity + 1)
            {
                return BadRequest("Stock is not enough");
            }

            basketItem.Quantity++;
            _context.SaveChanges();

            return Ok("Successfully incremented");
        }

        [HttpPost]
        public IActionResult DecrementItem(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;

            if (user == null)
            {
                return Unauthorized();
            }

            var basket = _context.Baskets.Include(b => b.Items).FirstOrDefault(x => x.UserId == user.Id);

            if (basket == null)
            {
                return NotFound();
            }

            var basketItem = basket.Items.FirstOrDefault(x => x.Id == id);

            if (basketItem == null)
            {
                return NotFound();
            }

            if (basketItem.Quantity == 1)
            {
                _context.BasketItems.Remove(basketItem);
                _context.SaveChanges();
                return Ok("Successfully removed");
            }

            basketItem.Quantity--;
            _context.SaveChanges();

            return Ok("Successfully decremented");
        }
    }
}
