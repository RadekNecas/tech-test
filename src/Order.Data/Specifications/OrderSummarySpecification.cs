using Order.Model;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Order.Data.Specifications
{
    public class OrderSummarySpecification : BaseEmptySpecification<Entities.Order, OrderSummary>
    {
        public override Expression<Func<Entities.Order, OrderSummary>> Select { get; }

        public OrderSummarySpecification() : base()
        {
            Includes = new Expression<Func<Entities.Order, object>>[]
            {
                x => x.Status,
                x => x.Items
            };

            Select = x => new OrderSummary
            {
                Id = new Guid(x.Id),
                ResellerId = new Guid(x.ResellerId),
                CustomerId = new Guid(x.CustomerId),
                StatusId = new Guid(x.StatusId),
                StatusName = x.Status.Name,
                ItemCount = x.Items.Count,
                TotalCost = x.Items.Sum(i => i.Quantity * i.Product.UnitCost).Value,
                TotalPrice = x.Items.Sum(i => i.Quantity * i.Product.UnitPrice).Value,
                CreatedDate = x.CreatedDate
            };

            OrderByDescending = x => x.CreatedDate;
        }
    }
}
