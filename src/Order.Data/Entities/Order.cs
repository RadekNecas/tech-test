using Order.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Order.Data.Entities
{
    public partial class Order
    {
        public Order()
        {
            Items = new HashSet<OrderItem>();
        }

        public byte[] Id { get; set; }
        public byte[] ResellerId { get; set; }
        public byte[] CustomerId { get; set; }
        public byte[] StatusId { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual OrderStatus Status { get; set; }
        public virtual ICollection<OrderItem> Items { get; set; }

        public OrderSummary AsOrderSummary()
        {
            return new OrderSummary
            {
                Id = new Guid(Id),
                ResellerId = new Guid(ResellerId),
                CustomerId = new Guid(CustomerId),
                StatusId = new Guid(StatusId),
                StatusName = Status.Name,
                ItemCount = Items.Count,
                TotalCost = Items.Sum(i => i.Quantity * i.Product.UnitCost).Value,
                TotalPrice = Items.Sum(i => i.Quantity * i.Product.UnitPrice).Value,
                CreatedDate = CreatedDate
            };
        }
    }
}
