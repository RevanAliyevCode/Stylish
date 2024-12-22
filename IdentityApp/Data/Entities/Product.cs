using System;

namespace StylishApp.Data.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageName { get; set; }
    public int Stock { get; set; }

    public ICollection<Color> Colors { get; set; }
    public ICollection<BasketItem> BasketItems { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
}
