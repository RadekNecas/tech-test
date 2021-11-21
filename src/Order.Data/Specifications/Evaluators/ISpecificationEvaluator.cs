using System.Linq;

namespace Order.Data.Specifications.Evaluators
{
    public interface ISpecificationEvaluator
    {
        IQueryable<TResult> EvaluateSpecification<TEntity, TResult>(ISpecification<TEntity, TResult> specification, IQueryable<TEntity> data) where TEntity : class;
        IQueryable<TEntity> EvaluateSpecification<TEntity>(ISpecification<TEntity> specification, IQueryable<TEntity> data) where TEntity : class;
    }
}