using Order.Data.Specifications;
using Order.Data.Specifications.Evaluators;
using System.Linq;

namespace Order.Data.Repositories
{
    public abstract class BaseRepository<TEntity> where TEntity: class
    {
        protected readonly OrderContext _orderContext;
        protected readonly ISpecificationEvaluator _specificationEvaluator;

        public BaseRepository(OrderContext orderContext, ISpecificationEvaluator specificationEvaluator)
        {
            _orderContext = orderContext;
            _specificationEvaluator = specificationEvaluator;
        }

        protected virtual IQueryable<TResult> EvaluateSpecification<TResult>(ISpecification<TEntity, TResult> specification)
        {
            return _specificationEvaluator.EvaluateSpecification(specification, _orderContext.Set<TEntity>());
        }

        protected virtual IQueryable<TEntity> EvaluateSpecification(ISpecification<TEntity> specification)
        {
            return _specificationEvaluator.EvaluateSpecification(specification, _orderContext.Set<TEntity>());
        }
    }
}
