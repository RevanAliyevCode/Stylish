using System;

namespace IdentityApp.Areas.Admin.Models.Users;

public class UsersVM
{
    public string Id { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
}
