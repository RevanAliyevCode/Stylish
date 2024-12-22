using IdentityApp.Data;
using IdentityApp.Utilities.Email;
using IdentityApp.Utilities.Email.Abstracts;
using IdentityApp.Utilities.Email.Concrets;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using StylishApp.Data.Entities;
using StylishApp.Models;
using StylishApp.Utilities.FileService;
using F = StylishApp.Utilities.FileService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();

var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddSingleton(emailConfig);

builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddSingleton<IFileService, F.FileService>();

var app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
              name: "areas",
              pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
            );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using var scope = app.Services.CreateScope();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
DbInitilazer.Seed(userManager, roleManager);

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Value;


app.Run();
