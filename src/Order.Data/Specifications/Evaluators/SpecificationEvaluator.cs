using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Order.Data.Specifications.Evaluators
{
    public class SpecificationEvaluator : ISpecificationEvaluator
    {
        public IQueryable<TResult> EvaluateSpecification<TEntity, TResult>(ISpecification<TEntity, TResult> specification, IQueryable<TEntity> data)
            where TEntity : class
        {
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

        public IQueryable<TEntity> EvaluateSpecification<TEntity>(ISpecification<TEntity, TEntity> specification, IQueryable<TEntity> data)
            where TEntity : class
        {
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
