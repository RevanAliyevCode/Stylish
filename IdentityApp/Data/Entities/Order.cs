using System;
using StylishApp.Data.Enums;

namespace StylishApp.Data.Entities;

public class Order : BaseEntity
{
    public OrderStatusEnum Status { get; set; }
    public Guid TrackingNumber { get; set; }
    public string UserId { get; set; }
    public AppUser User { get; set; }
    public List<OrderItem> Items { get; set; }
}
