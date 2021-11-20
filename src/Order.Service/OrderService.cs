using Order.Data;
using Order.Model;
using Order.Service.Specifications;
using System;
using System.Collections.Generic;
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
            IEnumerable<OrderSummary> orders;
            if(specification != null && specification.Status != null)
            {
                orders = await _orderRepository.GetOrdersAsync(specification.Status.Trim());
            }
            else
            {
                orders = await _orderRepository.GetOrdersAsync();
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
