using System;
using E = StylishApp.Data.Entities;

namespace StylishApp.Areas.Admin.Models.Product;

public class ProductVM
{
    public List<E.Product> Products { get; set; }
}
