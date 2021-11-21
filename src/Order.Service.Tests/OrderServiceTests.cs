﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using NUnit.Framework;
using Order.Data;
using Order.Data.Entities;
using Order.Data.Specifications.Evaluators;
using Order.Model;
using Order.Service.Specifications;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Service.Tests
{
    public class OrderServiceTests
    {
        private IOrderService _orderService;
        private Mock<IDateTimeProvider> _dateTimeProviderMock;
        private SpecificationEvaluator _specificationEvaluator;
        private IOrderRepository _orderRepository;
        private IOrderStatusRepository _orderStatusRepository;
        private IProductRepository _productRepository;
        private DbConnection _connection;
        private OrderContext _orderContext;

        private readonly Guid _orderStatusCreatedId = Guid.NewGuid();
        private readonly Guid _orderStatusInProgressId = Guid.NewGuid();
        private readonly Guid _orderStatusCompletedId = Guid.NewGuid();

        private readonly byte[] _orderServiceEmailId = Guid.NewGuid().ToByteArray();
        private readonly byte[] _orderProductEmailId = Guid.NewGuid().ToByteArray();

        private Dictionary<Guid, string> _orderStatuses;

        private DateTime DefaultCurrentUtcDate => new DateTime(2021, 11, 21, 15, 0, 0);
        

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<OrderContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .EnableDetailedErrors(true)
                .EnableSensitiveDataLogging(true)
                .Options;

            _connection = RelationalOptionsExtension.Extract(options).Connection;

            _orderContext = new OrderContext(options);
            _orderContext.Database.EnsureDeleted();
            _orderContext.Database.EnsureCreated();

            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _dateTimeProviderMock.Setup(m => m.GetCurrentUtcDate()).Returns(DefaultCurrentUtcDate);

            _specificationEvaluator = new SpecificationEvaluator();
            _orderRepository = new OrderRepository(_orderContext, _specificationEvaluator);
            _orderStatusRepository = new OrderStatusRepository(_orderContext, _specificationEvaluator);
            _productRepository = new ProductRepository(_orderContext, _specificationEvaluator);
            _orderService = new OrderService(_orderRepository, _orderStatusRepository, _productRepository, _dateTimeProviderMock.Object);

            _orderStatuses = new Dictionary<Guid, string>
            {
                { _orderStatusCreatedId, "Created" },
                { _orderStatusInProgressId, "InProgress" },
                { _orderStatusCompletedId, "Completed" },
            };

            await AddReferenceDataAsync(_orderContext);
        }

        [TearDown]
        public void TearDown()
        {
            _connection.Dispose();
            _orderContext.Dispose();
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            return connection;
        }

        [Test]
        public async Task GetOrdersAsync_ReturnsCorrectNumberOfOrders()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1, _orderStatusCreatedId);

            var orderId2 = Guid.NewGuid();
            await AddOrder(orderId2, 2, _orderStatusCreatedId);

            var orderId3 = Guid.NewGuid();
            await AddOrder(orderId3, 3, _orderStatusCreatedId);

            // Act
            var orders = await _orderService.GetOrdersAsync();

            // Assert
            Assert.AreEqual(3, orders.Count());
        }

        [Test]
        public async Task GetOrdersAsync_ReturnsOrdersWithCorrectTotals()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1, _orderStatusCreatedId);

            var orderId2 = Guid.NewGuid();
            await AddOrder(orderId2, 2, _orderStatusCreatedId);

            var orderId3 = Guid.NewGuid();
            await AddOrder(orderId3, 3, _orderStatusCreatedId);

            // Act
            var orders = await _orderService.GetOrdersAsync();

            // Assert
            var order1 = orders.SingleOrDefault(x => x.Id == orderId1);
            var order2 = orders.SingleOrDefault(x => x.Id == orderId2);
            var order3 = orders.SingleOrDefault(x => x.Id == orderId3);

            Assert.AreEqual(0.8m, order1.TotalCost);
            Assert.AreEqual(0.9m, order1.TotalPrice);

            Assert.AreEqual(1.6m, order2.TotalCost);
            Assert.AreEqual(1.8m, order2.TotalPrice);

            Assert.AreEqual(2.4m, order3.TotalCost);
            Assert.AreEqual(2.7m, order3.TotalPrice);
        }

        [Test]
        public async Task GetOrderByIdAsync_ReturnsCorrectOrder()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1, _orderStatusCreatedId);

            // Act
            var order = await _orderService.GetOrderByIdAsync(orderId1);

            // Assert
            Assert.AreEqual(orderId1, order.Id);
        }

        [Test]
        public async Task GetOrderByIdAsync_ReturnsCorrectOrderItemCount()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1, _orderStatusCreatedId);

            // Act
            var order = await _orderService.GetOrderByIdAsync(orderId1);

            // Assert
            Assert.AreEqual(1, order.Items.Count());
        }

        [Test]
        public async Task GetOrderByIdAsync_ReturnsOrderWithCorrectTotals()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 2, _orderStatusCreatedId);

            // Act
            var order = await _orderService.GetOrderByIdAsync(orderId1);

            // Assert
            Assert.AreEqual(1.6m, order.TotalCost);
            Assert.AreEqual(1.8m, order.TotalPrice);
        }

        [Test]
        public async Task GetOrders_UseSpecificationWithExistingStatusFilter_OnlyRecordsWithCorrectStatusReturned()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            await AddOrder(orderId1, 1, _orderStatusCreatedId);

            var orderId2 = Guid.NewGuid();
            await AddOrder(orderId2, 2, _orderStatusInProgressId);

            var orderId3 = Guid.NewGuid();
            await AddOrder(orderId3, 3, _orderStatusInProgressId);

            var expectedReturnedOrderIds = new[] { orderId2, orderId3 };

            // Act
            var orders = await _orderService.GetOrdersAsync(new ListOrdersSpecification(_orderStatuses[_orderStatusInProgressId]));

            // Assert
            Assert.AreEqual(orders.Count(), 2, "There should be only two items returned.");
            int i = 0;
            foreach(var order in orders)
            {
                Assert.AreEqual(_orderStatusInProgressId, order.StatusId, $"Invalid StatusId for Order at position {i}");
                Assert.AreEqual(_orderStatuses[_orderStatusInProgressId], order.StatusName, $"Invalid StatusName for Order at position {i}");
                Assert.Contains(order.Id, expectedReturnedOrderIds, $"Invalid order returned.");
                i++;
            }
        }

        [Test]
        public async Task GetOrders_UseSpecificationWithExistingStatusWithSpacesArroundFilter_RecordForTrimmedStatusReturned()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            var createdStatusName = _orderStatuses[_orderStatusCreatedId];
            await AddOrder(orderId1, 1, _orderStatusCreatedId);

            await AddOrder(Guid.NewGuid(), 2, _orderStatusInProgressId);
            await AddOrder(Guid.NewGuid(), 3, _orderStatusInProgressId);

            var status = $" \t {createdStatusName} \t \r\n \n  ";

            // Act
            var orders = await _orderService.GetOrdersAsync(new ListOrdersSpecification(status));

            // Assert
            Assert.AreEqual(orders.Count(), 1, "There should be only one item returned.");

            var returnedOrder = orders.First();
            Assert.AreEqual(orderId1, returnedOrder.Id, "Unexpected OrderId");
            Assert.AreEqual(_orderStatusCreatedId, returnedOrder.StatusId, "Unexpected StatusId");
            Assert.AreEqual(createdStatusName, returnedOrder.StatusName, "Unexpected StatusName");
        }

        [Test]
        public async Task GetOrders_UseSpecificationWithNotExistingStatusFilter_EmptyReturned()
        {
            // Arrange
            await AddOrder(Guid.NewGuid(), 1, _orderStatusCreatedId);
            await AddOrder(Guid.NewGuid(), 2, _orderStatusInProgressId);
            await AddOrder(Guid.NewGuid(), 3, _orderStatusInProgressId);

            var notExistingOrderStatus = string.Join("-", _orderStatuses.Values);

            // Act
            var orders = await _orderService.GetOrdersAsync(new ListOrdersSpecification(notExistingOrderStatus));

            // Assert
            Assert.IsEmpty(orders, "Empty collection should be returned for not existing status.");
        }

        [Test]
        public async Task GetOrders_UseSpecificationWithEmptyStrungStatus_EmptyReturned()
        {
            // Arrange
            await AddOrder(Guid.NewGuid(), 1, _orderStatusCreatedId);
            await AddOrder(Guid.NewGuid(), 2, _orderStatusInProgressId);
            await AddOrder(Guid.NewGuid(), 3, _orderStatusInProgressId);

            // Act
            var orders = await _orderService.GetOrdersAsync(new ListOrdersSpecification(string.Empty));

            // Assert
            Assert.IsEmpty(orders, "Empty collection should be returned for empty string as a status Filter. There is no status with empty string as a name in the database. If user does not want to filter by status he has to use null as a Specification or null as a Status in Specification.");
        }

        [Test]
        public async Task GetOrders_UseNullSpecification_ResultSameAsWithoutSpecification()
        {
            // Arrange
            await AddOrder(Guid.NewGuid(), 1, _orderStatusCreatedId);
            await AddOrder(Guid.NewGuid(), 2, _orderStatusInProgressId);
            await AddOrder(Guid.NewGuid(), 3, _orderStatusInProgressId);

            // Act
            var ordersNullSpecification = (await _orderService.GetOrdersAsync(null)).ToList();
            var ordersWithoutSpecification = (await _orderService.GetOrdersAsync()).ToList();

            // Assert
            Assert.AreEqual(ordersNullSpecification.Count(), ordersWithoutSpecification.Count(), "There should be exactly same amount of orders in both collections");
            for(int i = 0; i < ordersNullSpecification.Count(); i++)
            {
                Model.OrderSummary nullOrder = ordersNullSpecification[i];
                var noSpecOrder = ordersWithoutSpecification[i];
                Assert.AreEqual(nullOrder.Id, noSpecOrder.Id, "Collections should be exactly the same including ordering. OrderId does not match.");
                Assert.AreEqual(nullOrder.StatusId, noSpecOrder.StatusId, "Collections should be exactly the same including ordering. StatusId does not match.");
                Assert.AreEqual(nullOrder.StatusName, noSpecOrder.StatusName, "Collections should be exactly the same including ordering. StatusName does not match.");
            }
        }

        [Test]
        public async Task GetOrders_UseSpecificationWithNullStatus_ResultSameAsWithoutSpecification()
        {
            // Arrange
            await AddOrder(Guid.NewGuid(), 1, _orderStatusCreatedId);
            await AddOrder(Guid.NewGuid(), 2, _orderStatusInProgressId);
            await AddOrder(Guid.NewGuid(), 3, _orderStatusInProgressId);

            var specWithNullStatus = new ListOrdersSpecification(null);

            // Act
            var ordersSpecificationWithNullStatus = (await _orderService.GetOrdersAsync(specWithNullStatus)).ToList();
            var ordersWithoutSpecification = (await _orderService.GetOrdersAsync()).ToList();

            // Assert
            Assert.AreEqual(ordersSpecificationWithNullStatus.Count(), ordersWithoutSpecification.Count(), "There should be exactly same amount of orders in both collections");
            for (int i = 0; i < ordersSpecificationWithNullStatus.Count(); i++)
            {
                Model.OrderSummary nullOrder = ordersSpecificationWithNullStatus[i];
                var noSpecOrder = ordersWithoutSpecification[i];
                Assert.AreEqual(nullOrder.Id, noSpecOrder.Id, "Collections should be exactly the same including ordering. OrderId does not match.");
                Assert.AreEqual(nullOrder.StatusId, noSpecOrder.StatusId, "Collections should be exactly the same including ordering. StatusId does not match.");
                Assert.AreEqual(nullOrder.StatusName, noSpecOrder.StatusName, "Collections should be exactly the same including ordering. StatusName does not match.");
            }
        }

        public static IEnumerable<object> UpdateNullParametersSource()
        {
            yield return new object[] { null };
            yield return new object[] { new OrderToUpdate { StatusName = null } };
        }

        [TestCaseSource(nameof(UpdateNullParametersSource))]
        public async Task UpdateOrderAsync_NullValuesForOrderToUpdate_ExceptionThrown(OrderToUpdate orderToUpdate)
        {
            // Arrange
            var orderId = Guid.NewGuid();
            await AddOrder(orderId, 1, _orderStatusCreatedId);

            // Act, Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _orderService.UpdateOrderAsync(orderId, orderToUpdate));
        }

        [Test]
        public async Task UpdateOrderAsync_OrderToUpdateWithNotExistingStatus_ExceptionThrown()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            await AddOrder(orderId, 1, _orderStatusCreatedId);

            var notExistingStatus = string.Join("-", _orderStatuses.Values);
            OrderToUpdate orderToUpdate = new OrderToUpdate { StatusName = notExistingStatus };

            // Act, Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _orderService.UpdateOrderAsync(orderId, orderToUpdate));
        }

        [Test]
        public async Task UpdateOrderAsync_NotExistingOrderId_NullReturned()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var notExistingOrderId = Guid.NewGuid();
            await AddOrder(orderId, 1, _orderStatusCreatedId);

            OrderToUpdate orderToUpdate = new OrderToUpdate { StatusName = _orderStatuses[_orderStatusInProgressId] };

            // Act
            var result = await _orderService.UpdateOrderAsync(notExistingOrderId, orderToUpdate);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task UpdateOrderAsync_CorrectParameters_UpdatedEntityReturned()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            await AddOrder(orderId, 1, _orderStatusCreatedId);

            var newStatus = _orderStatuses[_orderStatusInProgressId];
            OrderToUpdate orderToUpdate = new OrderToUpdate { StatusName = newStatus };

            // Act
            var result = await _orderService.UpdateOrderAsync(orderId, orderToUpdate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(orderId, result.Id, "Incorrect result returned. OrderId does not match updated Order.");
            Assert.AreEqual(newStatus, result.StatusName, "Result StatusName property does not match updated value.");
            Assert.AreEqual(_orderStatusInProgressId, result.StatusId, "Result StatusId property does not match updated value.");
        }

        [Test]
        public async Task UpdateOrderAsync_CorrectParameters_EntityCorrectlyUpdated()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            await AddOrder(orderId, 1, _orderStatusCreatedId);

            var newStatus = _orderStatuses[_orderStatusInProgressId];
            OrderToUpdate orderToUpdate = new OrderToUpdate { StatusName = newStatus };

            // Act
            await _orderService.UpdateOrderAsync(orderId, orderToUpdate);

            // Assert
            var expectedUpdated = await _orderRepository.GetOrderByIdAsync(orderId);
            Assert.AreEqual(newStatus, expectedUpdated.StatusName, "StatusName is not correctly updated.");
            Assert.AreEqual(_orderStatusInProgressId, expectedUpdated.StatusId, "StatusId is not correctly updated.");
        }

        [Test]
        public async Task UpdateOrderAsync_CorrectParameters_OtherEntitiesUnchanged()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            await AddOrder(orderId, 1, _orderStatusCreatedId);

            var orderId2 = Guid.NewGuid();
            await AddOrder(orderId2, 1, _orderStatusCreatedId);

            var newStatus = _orderStatuses[_orderStatusInProgressId];
            OrderToUpdate orderToUpdate = new OrderToUpdate { StatusName = newStatus };

            // Act
            await _orderService.UpdateOrderAsync(orderId, orderToUpdate);

            // Assert
            var expectedNotUpdated = await _orderRepository.GetOrderByIdAsync(orderId2);
            Assert.AreEqual(_orderStatuses[_orderStatusCreatedId], expectedNotUpdated.StatusName, "StatusName should not be changed in not updated entity.");
            Assert.AreEqual(_orderStatusCreatedId, expectedNotUpdated.StatusId, "StatusId should not be changed in not updated entity.");
        }

        private async Task AddOrder(Guid orderId, int quantity, Guid statusId)
        {
            var orderIdBytes = orderId.ToByteArray();
            _orderContext.Order.Add(new Data.Entities.Order
            {
                Id = orderIdBytes,
                ResellerId = Guid.NewGuid().ToByteArray(),
                CustomerId = Guid.NewGuid().ToByteArray(),
                CreatedDate = DateTime.Now,
                StatusId = statusId.ToByteArray(),
            });

            _orderContext.OrderItem.Add(new Data.Entities.OrderItem
            {
                Id = Guid.NewGuid().ToByteArray(),
                OrderId = orderIdBytes,
                ServiceId = _orderServiceEmailId,
                ProductId = _orderProductEmailId,
                Quantity = quantity
            });

            await _orderContext.SaveChangesAsync();
        }

        private async Task AddReferenceDataAsync(OrderContext orderContext)
        {
            foreach (var keyValue in _orderStatuses)
            {
                orderContext.OrderStatus.Add(new OrderStatus
                {
                    Id = keyValue.Key.ToByteArray(),
                    Name = keyValue.Value,
                });
            }

            orderContext.OrderService.Add(new Data.Entities.OrderService
            {
                Id = _orderServiceEmailId,
                Name = "Email"
            });

            orderContext.OrderProduct.Add(new OrderProduct
            {
                Id = _orderProductEmailId,
                Name = "100GB Mailbox",
                UnitCost = 0.8m,
                UnitPrice = 0.9m,
                ServiceId = _orderServiceEmailId
            });

            await orderContext.SaveChangesAsync();
        }
    }
}
