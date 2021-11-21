using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Order.Data.Specifications
{
    public abstract class BaseEmptySpecification<TEntity, TResult> : ISpecification<TEntity, TResult> where TEntity: class
    {
        public virtual Expression<Func<TEntity, bool>> Query { get; protected set; } = null;
        public abstract Expression<Func<TEntity, TResult>> Select { get; }

        public virtual Expression<Func<TEntity, object>> OrderBy { get; protected set; } = null;
        public virtual Expression<Func<TEntity, object>> OrderByDescending { get; protected set; } = null;

        public bool HasQuery => Query != null;

        public bool HasSelect => Select != null;

        public bool HasOrderBy => OrderBy != null;

        public bool HasOrderByDescending => OrderByDescending != null;

        public IEnumerable<Expression<Func<TEntity, object>>> Includes { get; protected set; } = Enumerable.Empty<Expression<Func<TEntity, object>>>();

    }


    public abstract class BaseEmptySpecification<TEntity> : ISpecification<TEntity> where TEntity : class
    {
        public virtual Expression<Func<TEntity, bool>> Query { get; protected set; } = null;

        public virtual Expression<Func<TEntity, object>> OrderBy { get; protected set; } = null;
        public virtual Expression<Func<TEntity, object>> OrderByDescending { get; protected set; } = null;

        public bool HasQuery => Query != null;

        public bool HasOrderBy => OrderBy != null;

        public bool HasOrderByDescending => OrderByDescending != null;

        public IEnumerable<Expression<Func<TEntity, object>>> Includes { get; protected set; } = Enumerable.Empty<Expression<Func<TEntity, object>>>();
    }
}
