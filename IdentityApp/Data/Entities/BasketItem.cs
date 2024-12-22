using System;

namespace StylishApp.Data.Entities;

public class BasketItem : BaseEntity
{
    public int BasketId { get; set; }
    public Basket Basket { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
