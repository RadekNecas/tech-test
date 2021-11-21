using Microsoft.EntityFrameworkCore;
using Order.Data.Entities;
using Order.Data.Specifications;
using Order.Data.Specifications.Evaluators;
using System.Threading.Tasks;

namespace Order.Data.Repositories
{
    public class OrderStatusRepository : BaseRepository<OrderStatus>, IOrderStatusRepository
    {
        public OrderStatusRepository(OrderContext orderContext, ISpecificationEvaluator specificationEvaluator) : base(orderContext, specificationEvaluator)
        {
        }

        public async Task<OrderStatus> GetOrderStatusAsync(ISpecification<OrderStatus> specification)
        {
            var query = EvaluateSpecification(specification);
            return await query.FirstOrDefaultAsync();
        }
    }
}
