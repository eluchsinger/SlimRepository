using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using SlimRepository.Interfaces;

namespace SlimRepository.EntityFrameworkCore.Test.Data
{
    class EmptySpecification<T> : SpecificationBase<T>
    {
        public EmptySpecification(Expression<Func<T, bool>> criteria) : base(criteria)
        {
            
        }
    }
}
