using System;
using Microsoft.AspNetCore.Identity;
using StylishApp.Data.Entities;

namespace StylishApp.Data.Entities;

public class AppUser : IdentityUser
{
    public bool SubscribeNewsletter { get; set; }
    public Basket? Basket { get; set; }
}
