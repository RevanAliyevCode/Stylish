using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Models.Account;

public class ResetPasswordVM
{
    public string Email { get; set; }
    public string Token { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm password is required")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }
}
