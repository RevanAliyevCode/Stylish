using System;

namespace StylishApp.Data.Entities;

public class Color : BaseEntity
{
    public string Name { get; set; }
    public string Code { get; set; }
    public ICollection<Product> Products { get; set; }
}
