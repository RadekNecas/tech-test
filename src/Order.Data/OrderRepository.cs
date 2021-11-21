using Microsoft.EntityFrameworkCore;
using Order.Data.Entities;
using Order.Data.Specifications;
using Order.Data.Specifications.Evaluators;
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
        private readonly ISpecificationEvaluator _specificationEvaluator;

        public OrderRepository(OrderContext orderContext, ISpecificationEvaluator specificationEvaluator)
        {
            _orderContext = orderContext;
            _specificationEvaluator = specificationEvaluator;
        }

        public async Task<IReadOnlyList<OrderSummary>> GetOrdersAsync()
        {
            var query = EvaluateSpecification(new OrderSummarySpecification());
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<IReadOnlyList<OrderSummary>> GetOrdersAsync(ISpecification<Entities.Order, OrderSummary> specification)
        {
            var query = EvaluateSpecification(specification);
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<OrderDetail> GetOrderByIdAsync(Guid orderId)
        {
            var query = EvaluateSpecification(new OrderDetailByIdSpecification(orderId, _orderContext.IsInMemoryDatabase()));
            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<OrderSummary> UpdateOrderStatus(Guid orderId, OrderStatus newStatus)
        {
            // I want to keep interface coherent and simple so I want to reuse OrderSummary as a response
            // I can reuse also OrderSummaryByIdSpecification
            var query = EvaluateSpecification(new OrderSummaryByIdSpecification(orderId, _orderContext.IsInMemoryDatabase()));
            var existingOrder = await query.FirstOrDefaultAsync();
            if (existingOrder == null)
            {
                return null;
            }

            UpdateEntity(newStatus, existingOrder);
            await _orderContext.SaveChangesAsync();

            // Updating DTO entity
            existingOrder.StatusId = new Guid(newStatus.Id);
            existingOrder.StatusName = newStatus.Name;

            return existingOrder;
        }


        private IQueryable<TResult> EvaluateSpecification<TResult>(ISpecification<Entities.Order, TResult> specification)
        {
            return _specificationEvaluator.EvaluateSpecification(specification, _orderContext.Set<Entities.Order>());
        }

        private IQueryable<Entities.Order> EvaluateSpecification(ISpecification<Entities.Order> specification)
        {
            return _specificationEvaluator.EvaluateSpecification(specification, _orderContext.Set<Entities.Order>());
        }

        /// <summary>
        /// I'm using attach to update if possible, because I don't want to send another request to load entity from database
        /// and I have verified that order with ID exists by previous steps.
        /// </summary>
        /// <param name="newStatus"></param>
        /// <param name="existingOrder"></param>
        /// <returns></returns>
        private void UpdateEntity(OrderStatus newStatus, OrderSummary existingOrder)
        {
            var orderIdBytes = existingOrder.Id.ToByteArray();
            var localEntity = _orderContext.Order.Local.FirstOrDefault(x => _orderContext.IsInMemoryDatabase() 
                                                            ? x.Id.SequenceEqual(orderIdBytes)
                                                            : x.Id == orderIdBytes);

            Entities.Order dbEntityToUpdate;
            if (localEntity == null)
            {
                dbEntityToUpdate = new Entities.Order { Id = existingOrder.Id.ToByteArray(), StatusId = newStatus.Id };
                _orderContext.Attach(dbEntityToUpdate);
            }
            else
            {
                dbEntityToUpdate = localEntity;
                localEntity.StatusId = newStatus.Id;
            }

            _orderContext.Entry(dbEntityToUpdate).Property(x => x.StatusId).IsModified = true;
        }
    }
}
