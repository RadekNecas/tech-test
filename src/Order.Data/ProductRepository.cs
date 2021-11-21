using Microsoft.EntityFrameworkCore;
using Order.Data.Entities;
using Order.Data.Specifications;
using Order.Data.Specifications.Evaluators;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly OrderContext _orderContext;
        private readonly ISpecificationEvaluator _specificationEvaluator;

        public ProductRepository(OrderContext orderContext, ISpecificationEvaluator specificationEvaluator)
        {
            _orderContext = orderContext;
            _specificationEvaluator = specificationEvaluator;
        }

        public async Task<IReadOnlyList<OrderProduct>> GetProducts(ISpecification<OrderProduct> specification)
        {
            var query = EvaluateSpecification(specification);
            return await query.ToListAsync();
        }

        private IQueryable<OrderProduct> EvaluateSpecification(ISpecification<OrderProduct> specification)
        {
            return _specificationEvaluator.EvaluateSpecification(specification, _orderContext.Set<OrderProduct>());
        }
    }
}

