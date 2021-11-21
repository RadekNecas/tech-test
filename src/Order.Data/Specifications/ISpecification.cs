using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Order.Data.Specifications
{
    public interface ISpecification <TEntity>
    {
        Expression<Func<TEntity, bool>> Query { get; }
        Expression<Func<TEntity, object>> OrderBy { get; }
        Expression<Func<TEntity, object>> OrderByDescending { get; }
        IEnumerable<Expression<Func<TEntity, object>>> Includes { get; }

        bool HasQuery { get; }
        bool HasOrderBy { get; }
        bool HasOrderByDescending { get; }
    }


    public interface ISpecification<TEntity, TResult> : ISpecification<TEntity>
    {
        Expression<Func<TEntity, TResult>> Select { get; }

        bool HasSelect { get; }
    }
}
