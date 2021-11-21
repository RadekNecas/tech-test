using Order.Model;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Order.Data.Specifications.OrderDetailSpec
{
    public class OrderDetailByIdSpecification : BaseEmptySpecification<Entities.Order, OrderDetail>
    {
        public override Expression<Func<Entities.Order, OrderDetail>> Select { get; }

        public OrderDetailByIdSpecification(Guid orderId, bool isInMemoryDatabase)
        {
            var orderIdBytes = orderId.ToByteArray();
            Query = x => isInMemoryDatabase ? x.Id.SequenceEqual(orderIdBytes) : x.Id == orderIdBytes;

            Includes = new Expression<Func<Entities.Order, object>>[]
            {
                x => x.Status,
                x => x.Items
            };

            Select = order => new OrderDetail
            {
                Id = new Guid(order.Id),
                ResellerId = new Guid(order.ResellerId),
                CustomerId = new Guid(order.CustomerId),
                StatusId = new Guid(order.StatusId),
                StatusName = order.Status.Name,
                CreatedDate = order.CreatedDate,
                TotalCost = order.Items.Sum(x => x.Quantity * x.Product.UnitCost).Value,
                TotalPrice = order.Items.Sum(x => x.Quantity * x.Product.UnitPrice).Value,
                Items = order.Items.Select(x => new OrderItem
                {
                    Id = new Guid(x.Id),
                    OrderId = new Guid(x.OrderId),
                    ServiceId = new Guid(x.ServiceId),
                    ServiceName = x.Service.Name,
                    ProductId = new Guid(x.ProductId),
                    ProductName = x.Product.Name,
                    UnitCost = x.Product.UnitCost,
                    UnitPrice = x.Product.UnitPrice,
                    TotalCost = x.Product.UnitCost * x.Quantity.Value,
                    TotalPrice = x.Product.UnitPrice * x.Quantity.Value,
                    Quantity = x.Quantity.Value
                })
            };
        }
    }
}
