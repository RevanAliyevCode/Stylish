using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityApp.Areas.Admin.Models.Users;

public class UpdateUserVM
{
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }

    public List<string>? RoleIds { get; set; }
    public List<SelectListItem>? Roles { get; set; }
}
