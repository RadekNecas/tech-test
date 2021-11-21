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
        private readonly IOrderStatusRepository _orderStatusRepository;

        public OrderService(IOrderRepository orderRepository, IOrderStatusRepository orderStatusRepository)
        {
            _orderRepository = orderRepository;
            _orderStatusRepository = orderStatusRepository;
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

        public async Task<OrderSummary> UpdateOrderAsync(Guid orderId, OrderToUpdate orderToUpdate)
        {
            if (orderToUpdate == null) throw new ArgumentNullException(nameof(orderToUpdate), "Specification must be set.");
            if(orderToUpdate.StatusName == null) throw new ArgumentNullException(nameof(orderToUpdate), "Specification attribute NewStatus must be set.");

            var newStatus = await _orderStatusRepository.GetOrderStatusAsync(new OrderStatusByNameSpecification(orderToUpdate.StatusName.Trim()));
            if(newStatus == null)
            {
                throw new InvalidOperationException($"Status with name {orderToUpdate.StatusName} does not exist");
            }

            // TODO: Here might be added other status validation like if it is possible to change to final status.
            // In case of multiple conditions I would think about implementation with usage of Chain of Responsibility pattern.

            return await _orderRepository.UpdateOrderStatus(orderId, newStatus);
        }
    }
}
