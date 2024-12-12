using System;
using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Data.Entities;

public class AppUser : IdentityUser
{
    public bool SubscribeNewsletter { get; set; }
}
