using System;

namespace StylishApp.Data.Entities;

public class Basket : BaseEntity
{
    public string UserId { get; set; }
    public AppUser User { get; set; }
    public ICollection<BasketItem> Items { get; set; }
}
