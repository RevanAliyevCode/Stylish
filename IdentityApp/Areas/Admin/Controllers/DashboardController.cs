using IdentityApp.Areas.Admin.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StylishApp.Data.Entities;


namespace ZayShopMVC.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class DashboardController : Controller
    {
        readonly UserManager<AppUser> _userManager;
        readonly RoleManager<IdentityRole> _roleManager;

        public DashboardController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var model = new List<UsersVM>();
            foreach (var user in _userManager.Users.ToList())
            {
                if (!_userManager.IsInRoleAsync(user, "Admin").Result)
                {
                    var userRoles = _userManager.GetRolesAsync(user).Result;
                    model.Add(new UsersVM
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Roles = userRoles.ToList()
                    });
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user != null)
            {
                return View(user);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateUserVM();
            var roles = _roleManager.Roles.ToList();
            model.Roles = roles.Where(r => r.Name != "Admin").Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id
            }).ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(CreateUserVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new AppUser
            {
                Email = model.Email,
                UserName = model.Email
            };
            var result = _userManager.CreateAsync(user, model.Password).Result;
            if (result.Succeeded)
            {
                foreach (var roleId in model.RoleIds)
                {
                    var role = _roleManager.FindByIdAsync(roleId).Result;
                    if (role != null)
                    {
                        _userManager.AddToRoleAsync(user, role.Name).Wait();
                    }
                }
                return RedirectToAction("Index");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Update(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user != null)
            {
                var model = new UpdateUserVM
                {
                    Email = user.Email
                };
                var roles = _roleManager.Roles.ToList();

                model.Roles = roles.Where(r => r.Name != "Admin").Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Id
                }).ToList();

                var userRoleNames = _userManager.GetRolesAsync(user).Result;
                model.RoleIds = roles
                    .Where(r => userRoleNames.Contains(r.Name))
                    .Select(r => r.Id)
                    .ToList();

                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Update(string id, UpdateUserVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _userManager.FindByIdAsync(id).Result;

            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View(model);
            }

            if (model.Email != null && model.Email != user.Email)
            {
                user.Email = model.Email;
                user.UserName = model.Email;
            }

            if (model.NewPassword != null)
            {
                var newPassword = _userManager.PasswordHasher.HashPassword(user, model.NewPassword);
                user.PasswordHash = newPassword;
            }

            var userRoles = new List<string>();

            foreach (var role in _roleManager.Roles.ToList())
            {
                if (_userManager.IsInRoleAsync(user, role.Name).Result)
                {
                    userRoles.Add(role.Id);
                }
            }

            var selectedRoles = model.RoleIds.Except(userRoles).ToList();
            var unselectedRoles = userRoles.Except(model.RoleIds).ToList();

            foreach (var roleId in selectedRoles)
            {
                var role = _roleManager.FindByIdAsync(roleId).Result;
                if (role != null)
                {
                    _userManager.AddToRoleAsync(user, role.Name).Wait();
                }
            }

            foreach (var roleId in unselectedRoles)
            {
                var role = _roleManager.FindByIdAsync(roleId).Result;
                if (role != null)
                {
                    _userManager.RemoveFromRoleAsync(user, role.Name).Wait();
                }
            }

            var result = _userManager.UpdateAsync(user).Result;
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;

            if (user != null)
            {
                var result = _userManager.DeleteAsync(user).Result;
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
