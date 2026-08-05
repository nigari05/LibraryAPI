using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DataAccess.Specification
{
    /// <summary>
    /// ISpecification üçün baza implementasiya. Törəyən siniflər AddCriteria(...) metodunu
    /// bir neçə dəfə çağıraraq şərtləri tədricən (AND məntiqi ilə) əlavə edə bilərlər -
    /// bu da hər filtrin ayrıca, oxunaqlı və test edilə bilən şəkildə tərtib olunmasına imkan verir.
    /// </summary>
    public class BaseSpecification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>>? Criteria { get; private set; }

        public List<string> IncludeStrings { get; } = new();

        public Expression<Func<T, object>>? OrderBy { get; private set; }

        public Expression<Func<T, object>>? OrderByDescending { get; private set; }

        public int Skip { get; private set; }

        public int Take { get; private set; }

        public bool IsPagingEnabled { get; private set; }

        /// <summary>
        /// Yeni bir şərti mövcud Criteria ilə AND məntiqi ilə birləşdirir.
        /// Heç bir şərt yoxdursa, sadəcə ilk şərt kimi təyin olunur.
        /// </summary>
        protected void AddCriteria(Expression<Func<T, bool>> criteria)
        {
            Criteria = Criteria == null ? criteria : CombineWithAnd(Criteria, criteria);
        }

        protected void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        private static Expression<Func<T, bool>> CombineWithAnd(
            Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftBody = new ParameterReplacer(left.Parameters[0], parameter).Visit(left.Body);
            var rightBody = new ParameterReplacer(right.Parameters[0], parameter).Visit(right.Body);

            var combined = Expression.AndAlso(leftBody!, rightBody!);
            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        /// <summary>
        /// İki fərqli lambda ifadəsinin parametrlərini eyniləşdirmək üçün istifadə olunur,
        /// ki, onlar tək bir Expression ağacında birləşdirilə bilsin.
        /// </summary>
        private sealed class ParameterReplacer : ExpressionVisitor
        {
            private readonly Expression _oldParameter;
            private readonly Expression _newParameter;

            public ParameterReplacer(Expression oldParameter, Expression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            public override Expression? Visit(Expression? node)
                => node == _oldParameter ? _newParameter : base.Visit(node);
        }
    }
}
