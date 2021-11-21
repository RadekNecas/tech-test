using Order.Data.Specifications;
using Order.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Order.Data
{
    public interface IOrderRepository
    {
        Task<IReadOnlyList<OrderSummary>> GetOrdersAsync();

        Task<OrderDetail> GetOrderByIdAsync(Guid orderId);

        Task<IReadOnlyList<OrderSummary>> GetOrdersAsync(ISpecification<Entities.Order, OrderSummary> specification);
    }
}
