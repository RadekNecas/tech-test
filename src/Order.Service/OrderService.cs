using Order.Data;
using Order.Data.Specifications;
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

        public async Task<IReadOnlyList<OrderSummary>> GetOrdersAsync(ListOrdersSpecification specification = null)
        {
            if(specification != null && specification.Status != null)
            {
                var spec = new OrderSummarysWithStatusOrderedByDateSpecification(specification.Status.Trim());
                return await _orderRepository.GetOrdersAsync(spec);
            }
            else
            {
                return await _orderRepository.GetOrdersAsync();
            }
        }

        public async Task<OrderDetail> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            return order;
        }
    }
}
