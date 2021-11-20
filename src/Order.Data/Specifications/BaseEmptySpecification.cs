using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Order.Data.Specifications
{
    public abstract class BaseEmptySpecification<T, TOrderingKey> : ISpecification<T, TOrderingKey>
    {
        public virtual Expression<Func<T, bool>> Query { get; protected set; } = null;
        public virtual Expression<Func<T, TOrderingKey>> OrderBy { get; protected set; } = null;
        public virtual Expression<Func<T, TOrderingKey>> OrderByDescending { get; protected set; } = null;

        public IEnumerable<Expression<Func<T, TProperty>>> GetIncludes<TProperty>()
            => throw new NotImplementedException();
    }
}
