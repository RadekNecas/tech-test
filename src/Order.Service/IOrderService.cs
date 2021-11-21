using Order.Model;
using Order.Service.Specifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Order.Service
{
    public interface IOrderService
    {
        Task<IReadOnlyList<OrderSummary>> GetOrdersAsync(ListOrdersSpecification specification = null);
        
        Task<OrderDetail> GetOrderByIdAsync(Guid orderId);
    }
}
