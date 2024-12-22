using System;
using System.ComponentModel.DataAnnotations;
using StylishApp.Data.Entities;

namespace StylishApp.Areas.Admin.Models.Product;

public class CreateProductVM
{
    [Required(ErrorMessage = "Please enter product name")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Please enter product description")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Please enter product price")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Please select product image")]
    public IFormFile ImageFile { get; set; }

    [Required(ErrorMessage = "Please enter product stock")]
    public int Stock { get; set; }


    [Display(Name = "Color")]
    [Required(ErrorMessage = "Please select product color")]
    public ICollection<int> ColorIds { get; set; }

    public List<Color>? AvailableColors { get; set; }
}
