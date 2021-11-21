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
            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<OrderSummary>> GetOrdersAsync(ISpecification<Entities.Order, OrderSummary> specification)
        {
            var query = EvaluateSpecification(specification);
            return await query.ToListAsync();
        }

        public async Task<OrderDetail> GetOrderByIdAsync(Guid orderId)
        {
            var query = EvaluateSpecification(new OrderDetailByIdSpecification(orderId, _orderContext.Database.IsInMemory()));
            return await query.FirstOrDefaultAsync();
        }

        private IQueryable<TResult> EvaluateSpecification<TResult>(ISpecification<Entities.Order, TResult> specification)
        {
            return _specificationEvaluator.EvaluateSpecification(specification, _orderContext.Set<Entities.Order>());
        }
    }
}
