using Microsoft.EntityFrameworkCore;
using Order.Data.Entities;
using Order.Data.Specifications;
using Order.Data.Specifications.Evaluators;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Data
{
    public class OrderStatusRepository : IOrderStatusRepository
    {
        private readonly OrderContext _orderContext;
        private readonly ISpecificationEvaluator _specificationEvaluator;

        public OrderStatusRepository(OrderContext orderContext, ISpecificationEvaluator specificationEvaluator)
        {
            _orderContext = orderContext;
            _specificationEvaluator = specificationEvaluator;
        }

        public async Task<OrderStatus> GetOrderStatusAsync(ISpecification<OrderStatus> specification)
        {
            var query = EvaluateSpecification(specification);
            return await query.FirstOrDefaultAsync();
        }

        private IQueryable<OrderStatus> EvaluateSpecification(ISpecification<OrderStatus> specification)
        {
            return _specificationEvaluator.EvaluateSpecification(specification, _orderContext.Set<OrderStatus>());
        }
    }
}
