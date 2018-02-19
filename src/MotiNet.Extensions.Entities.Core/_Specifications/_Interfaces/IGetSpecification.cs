using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public interface IGetSpecification<TEntity>
        where TEntity : class
    {
        ICollection<Expression<Func<TEntity, object>>> Includes { get; }

        ICollection<string> IncludeStrings { get; }

        ICollection<ManyToManyIncludeSpecification<TEntity>> ManyToManyIncludes { get; }

        void AddInclude(Expression<Func<TEntity, object>> includeExpression);

        void AddInclude(string includeString);

        void AddInclude(ManyToManyIncludeSpecification<TEntity> manyToManyInclude);
    }
}
