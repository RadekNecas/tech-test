using Microsoft.EntityFrameworkCore;
using Order.Data.Entities;
using Order.Data.Specifications;
using Order.Data.Specifications.Evaluators;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Order.Data.Repositories
{
    public class ProductRepository : BaseRepository<OrderProduct>, IProductRepository
    {
        public ProductRepository(OrderContext orderContext, ISpecificationEvaluator specificationEvaluator) : base(orderContext, specificationEvaluator)
        {
        }

        public async Task<IReadOnlyList<OrderProduct>> GetProducts(ISpecification<OrderProduct> specification)
        {
            var query = EvaluateSpecification(specification);
            return await query.ToListAsync();
        }
    }
}

