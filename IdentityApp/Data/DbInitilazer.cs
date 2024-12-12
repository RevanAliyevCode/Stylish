using System;
using IdentityApp.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Data;

public static class DbInitilazer
{
    public static void Seed(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        SeedRoles(roleManager);
        SeedAdmin(userManager, roleManager);
    }

    private static void SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in Enum.GetNames<RolesEnum>())
        {
            if (!roleManager.RoleExistsAsync(role).Result)
            {
                var identityRole = new IdentityRole { Name = role };
                var result = roleManager.CreateAsync(identityRole).Result;
                if (!result.Succeeded)
                {
                    throw new Exception("Cannot create role: " + result.Errors.FirstOrDefault()?.Description);
                }
            }
        }
    }

    private static void SeedAdmin(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        if (userManager.FindByEmailAsync("admin@gmail.com").Result == null)
        {
            var admin = new AppUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
            };

            var result = userManager.CreateAsync(admin, "Admin123-").Result;
            if (!result.Succeeded)
                throw new Exception("Cannot create admin: " + result.Errors.FirstOrDefault()?.Description);
            
            var role = roleManager.FindByNameAsync(RolesEnum.Admin.ToString()).Result;
            if (role?.Name == null)
                throw new Exception("Cannot find role: " + RolesEnum.Admin);

            result = userManager.AddToRoleAsync(admin, role.Name).Result;

            if (!result.Succeeded)
                throw new Exception("Cannot add user to role: " + result.Errors.FirstOrDefault()?.Description);
        }
    }
}
