using System;
using IdentityApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using StylishApp.Data.Entities;
using StylishApp.Models;

namespace StylishApp.Controllers;

public class PaymentController : Controller
{
    readonly IdentityContext _context;
    readonly UserManager<AppUser> _userManager;

    public PaymentController(IdentityContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCheckoutSession()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        user = await _context.Users.Include(x => x.Basket).ThenInclude(y => y.Items).ThenInclude(z => z.Product).FirstOrDefaultAsync(x => x.Id == user.Id);


        var order = new Order
        {
            UserId = user.Id,
            Status = Data.Enums.OrderStatusEnum.Pending,
            TrackingNumber = Guid.NewGuid(),
        };

        _context.Orders.Add(order);

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string>
            {
                "card",
            },
            Mode = "payment",
            SuccessUrl = Url.Action("Success", "Payment", new { trackId = order.TrackingNumber }, Request.Scheme),
            CancelUrl = Url.Action("Cancel", "Payment", new { trackId = order.TrackingNumber }, Request.Scheme),
        };

        var items = new List<SessionLineItemOptions>();
        foreach (var item in user.Basket.Items)
        {
            var orderItem = new OrderItem
            {
                Order = order,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price,
            };

            if (item.Product.Stock < item.Quantity)
            {
                return BadRequest(new { error = "Not enough stock" });
            }

            _context.OrderItems.Add(orderItem);
            items.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Product.Name,
                    },
                    UnitAmount = (long)(item.Product.Price * 100),
                },
                Quantity = item.Quantity,
            });

        }

        items.Add(new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "usd",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = "Delivery Fee",
                },
                UnitAmount = 500,
            },
            Quantity = 1,
        });

        options.LineItems = items;

        try
        {
            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            _context.SaveChanges();
            return Json(new { id = session.Id });
        }
        catch (StripeException e)
        {
            return BadRequest(new { error = e.Message });
        }

    }

    public IActionResult Success(Guid trackId)
    {
        var user = _userManager.GetUserAsync(User).Result;
        if (user == null)
            return Unauthorized();

        user = _context.Users.Include(x => x.Basket).ThenInclude(y => y.Items).ThenInclude(z => z.Product).FirstOrDefault(x => x.Id == user.Id);

        var order = _context.Orders.Include(o => o.Items).ThenInclude(oi => oi.Product).FirstOrDefault(x => x.TrackingNumber == trackId);

        if (order == null)
            return NotFound();

        order.Status = Data.Enums.OrderStatusEnum.Processing;

        foreach (var item in order.Items)
        {
            item.Product.Stock -= item.Quantity;
        }

        user.Basket.Items.Clear();

        _context.SaveChanges();
        return View();
    }

    public IActionResult Cancel(Guid trackId)
    {
        return View();
    }
}
