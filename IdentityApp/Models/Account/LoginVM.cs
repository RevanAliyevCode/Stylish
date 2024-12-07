using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Models.Account;

public class LoginVM
{
    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public string? ReturnUrl { get; set; }
}
