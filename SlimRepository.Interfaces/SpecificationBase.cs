using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SlimRepository.Interfaces
{
    public abstract class SpecificationBase<T> : ISpecification<T>
    {
        protected SpecificationBase(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<T, bool>> Criteria { get; }
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();

        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        /// <summary>
        /// Allows for including children of children, e.g. Organisation.Department.Team
        /// </summary>
        /// <param name="includeString">The string containing the hierarchy to include.</param>
        protected virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }
    }
}
