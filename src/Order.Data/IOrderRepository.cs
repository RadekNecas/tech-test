using Order.Data.Specifications;
using Order.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Order.Data
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderSummary>> GetOrdersAsync();

        Task<IEnumerable<OrderSummary>> GetOrdersAsync(string status);

        Task<OrderDetail> GetOrderByIdAsync(Guid orderId);
        Task<IEnumerable<OrderSummary>> GetOrdersAsync(ISpecification<Entities.Order, OrderSummary> specification);
    }
}
