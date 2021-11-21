using Order.Data;
using Order.Data.Entities;
using Order.Data.Specifications;
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
        private readonly IOrderStatusRepository _orderStatusRepository;
        private readonly IProductRepository _productRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public OrderService(IOrderRepository orderRepository, IOrderStatusRepository orderStatusRepository, IProductRepository productRepository, IDateTimeProvider dateTimeProvider)
        {
            _orderRepository = orderRepository;
            _orderStatusRepository = orderStatusRepository;
            _productRepository = productRepository;
            _dateTimeProvider = dateTimeProvider;
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

            return await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus);
        }

        public async Task<OrderSummary> AddOrderAsync(AddOrder orderToAdd)
        {
            if (orderToAdd == null) throw new ArgumentNullException(nameof(orderToAdd), "Order to add must be specified");

            var orderProductNames = orderToAdd.Items.Select(i => i.ProductName.Trim()).Distinct().ToList();
            var existingProducts = (await _productRepository.GetProducts(new ProductsByNamesSpecification(orderProductNames))).ToDictionary(p => p.Name);

            if(orderProductNames.Count() != existingProducts.Count())
            {
                var missingProducts = orderProductNames.Where(n => !existingProducts.ContainsKey(n)).ToList();
                throw new InvalidOperationException($"Only already existing product can be used to create order. Products that don't exist: '{string.Join(",", missingProducts)}'");
            }

            var createdStatus = await _orderStatusRepository.GetOrderStatusAsync(new OrderStatusByNameSpecification("Created"));

            var newOrder = new Data.Entities.Order
            {
                Id = Guid.NewGuid().ToByteArray(),
                ResellerId = orderToAdd.ResellerId.ToByteArray(),
                CustomerId = orderToAdd.CustomerId.ToByteArray(),
                CreatedDate = _dateTimeProvider.GetCurrentUtcDate(),
                Status = createdStatus,
                StatusId = createdStatus.Id
            };

            newOrder.Items = orderToAdd.Items.Select(addItem => new Data.Entities.OrderItem
            {
                Id = Guid.NewGuid().ToByteArray(),
                Order = newOrder,
                OrderId = newOrder.Id,
                Product = existingProducts[addItem.ProductName],
                ProductId = existingProducts[addItem.ProductName].Id,
                Service = existingProducts[addItem.ProductName].Service,
                ServiceId = existingProducts[addItem.ProductName].Service.Id,
                Quantity = addItem.Quantity
            }).ToList();

            await _orderRepository.AddOrderAsync(newOrder);

            return newOrder.AsOrderSummary();
        }
    }
}
