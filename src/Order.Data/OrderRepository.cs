using Microsoft.EntityFrameworkCore;
using Order.Data.Entities;
using Order.Data.Specifications;
using Order.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderContext _orderContext;

        public OrderRepository(OrderContext orderContext)
        {
            _orderContext = orderContext;
        }

        public async Task<IEnumerable<OrderSummary>> GetOrdersAsync()
        {
            var query = EvaluateSpecification(new OrderSummarySpecification());
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<OrderSummary>> GetOrdersAsync(ISpecification<Entities.Order, OrderSummary> specification)
        {
            var query = EvaluateSpecification(specification);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<OrderSummary>> GetOrdersAsync(string status)
        {
            var orders = await GetOrdersAsync();

            return orders.Where(o => o.StatusName.Equals(status));
        }

        public async Task<OrderDetail> GetOrderByIdAsync(Guid orderId)
        {
            //var orderIdBytes = orderId.ToByteArray();

            //var order = await _orderContext.Order.SingleOrDefaultAsync(x => _orderContext.Database.IsInMemory() ? x.Id.SequenceEqual(orderIdBytes) : x.Id == orderIdBytes);
            //if (order == null)
            //{
            //    return null;
            //}

            //var orderDetail = new OrderDetail
            //{
            //    Id = new Guid(order.Id),
            //    ResellerId = new Guid(order.ResellerId),
            //    CustomerId = new Guid(order.CustomerId),
            //    StatusId = new Guid(order.StatusId),
            //    StatusName = order.Status.Name,
            //    CreatedDate = order.CreatedDate,
            //    TotalCost = order.Items.Sum(x => x.Quantity * x.Product.UnitCost).Value,
            //    TotalPrice = order.Items.Sum(x => x.Quantity * x.Product.UnitPrice).Value,
            //    Items = order.Items.Select(x => new Model.OrderItem
            //    {
            //        Id = new Guid(x.Id),
            //        OrderId = new Guid(x.OrderId),
            //        ServiceId = new Guid(x.ServiceId),
            //        ServiceName = x.Service.Name,
            //        ProductId = new Guid(x.ProductId),
            //        ProductName = x.Product.Name,
            //        UnitCost = x.Product.UnitCost,
            //        UnitPrice = x.Product.UnitPrice,
            //        TotalCost = x.Product.UnitCost * x.Quantity.Value,
            //        TotalPrice = x.Product.UnitPrice * x.Quantity.Value,
            //        Quantity = x.Quantity.Value
            //    })
            //};

            //return orderDetail;

            var query = EvaluateSpecification(new OrderDetailByIdSpecification(orderId, _orderContext.Database.IsInMemory()));
            return await query.FirstOrDefaultAsync();
        }

        private IQueryable<TResult> EvaluateSpecification<TResult>(ISpecification<Entities.Order, TResult> specification)
        {
            IQueryable<Entities.Order> data = _orderContext.Set<Entities.Order>();

            if (specification.HasQuery)
            {
                data = data.Where(specification.Query);
            }

            if (specification.HasOrderBy)
            {
                data = data.OrderBy(specification.OrderBy);
            }

            if (specification.HasOrderByDescending)
            {
                data = data.OrderByDescending(specification.OrderByDescending);
            }

            foreach (var includeClause in specification.Includes)
            {
                data = data.Include(includeClause);
            }

            return data.Select(specification.Select);
        }

        private IQueryable<Entities.Order> EvaluateSpecification(ISpecification<Entities.Order, Entities.Order> specification)
        {
            IQueryable<Entities.Order> data = _orderContext.Set<Entities.Order>();

            if (specification.HasQuery)
            {
                data = data.Where(specification.Query);
            }

            if (specification.HasOrderBy)
            {
                data = data.OrderBy(specification.OrderBy);
            }

            if (specification.HasOrderByDescending)
            {
                data = data.OrderByDescending(specification.OrderByDescending);
            }

            foreach (var includeClause in specification.Includes)
            {
                data = data.Include(includeClause);
            }

            return data;
        }
    }
}
