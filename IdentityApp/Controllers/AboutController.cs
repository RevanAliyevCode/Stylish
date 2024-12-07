using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    [Authorize(Roles = "Admin, Customer")]
    public class AboutController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
