using Microsoft.EntityFrameworkCore;
using Order.Data.Entities;
using Order.Data.Specifications;
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

        public OrderRepository(OrderContext orderContext)
        {
            _orderContext = orderContext;
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
            IQueryable<Entities.Order> data = _orderContext.Set<Entities.Order>();

            if (specification.HasQuery)
            {
                data = data.Where(specification.Query);
            }

            if (specification.HasOrderBy)
            {
                data = data.OrderBy(specification.OrderBy);
            }

            if (specification.HasOrderByDescending)
            {
                data = data.OrderByDescending(specification.OrderByDescending);
            }

            foreach (var includeClause in specification.Includes)
            {
                data = data.Include(includeClause);
            }

            return data.Select(specification.Select);
        }

        private IQueryable<Entities.Order> EvaluateSpecification(ISpecification<Entities.Order, Entities.Order> specification)
        {
            IQueryable<Entities.Order> data = _orderContext.Set<Entities.Order>();

            if (specification.HasQuery)
            {
                data = data.Where(specification.Query);
            }

            if (specification.HasOrderBy)
            {
                data = data.OrderBy(specification.OrderBy);
            }

            if (specification.HasOrderByDescending)
            {
                data = data.OrderByDescending(specification.OrderByDescending);
            }

            foreach (var includeClause in specification.Includes)
            {
                data = data.Include(includeClause);
            }

            return data;
        }
    }
}
