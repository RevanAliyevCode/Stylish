using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace IdentityApp.Controllers;

[Authorize(Roles = "Admin, Seller")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
