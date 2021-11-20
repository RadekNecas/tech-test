using Order.Data;
using Order.Model;
using Order.Service.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderSummary>> GetOrdersAsync(OrderSpecification specification = null)
        {
            var orders = await _orderRepository.GetOrdersAsync();
            if(specification != null && specification.Status != null)
            {
                orders = orders.Where(o => o.StatusName.Equals(specification.Status.Trim()));
            }

            return orders;
        }

        public async Task<OrderDetail> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            return order;
        }
    }
}
