using IdentityApp.Areas.Admin.Models.Users;
using IdentityApp.Data.Entities;
using IdentityApp.Utilities.Email.Abstracts;
using IdentityApp.Utilities.Email.Concrets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SendNewsController : Controller
    {
        readonly UserManager<AppUser> _userManager;
        readonly IEmailSender _emailSender;

        public SendNewsController(UserManager<AppUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public ActionResult Index()
        {
            var model = _userManager.Users.Where(u => u.SubscribeNewsletter).Select(u => new UsersVM { Email = u.Email }).ToList();
            return View(model);
        }

        public IActionResult SendMail()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendMail(string message)
        {
            var users = _userManager.Users.Where(u => u.SubscribeNewsletter).Select(e => e.Email).ToList();

            try
            {
                var link = Url.Action("Index", "Home", new { area = "" }, Request.Scheme);
                _emailSender.SendEmail(new Message(users, "News", $"{message} {link}"));
            }
            catch
            {
                ViewData["ErrorMessage"] = "Error sending email";
                return View();
            }

            return RedirectToAction("Index");
        }
    }
}
