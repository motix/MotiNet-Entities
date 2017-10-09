using System;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public class OneToManyRelationshipSpecification<TEntity>
        where TEntity : class
    {
        public OneToManyRelationshipSpecification(
            Expression<Func<TEntity, object>> foreignKeyExpression,
            Expression<Func<TEntity, object>> parentExpression,
            Expression<Func<object, object>> parentIdExpression)
        {
            ForeignKeyExpression = foreignKeyExpression ?? throw new ArgumentNullException(nameof(foreignKeyExpression));
            ParentExpression = parentExpression ?? throw new ArgumentNullException(nameof(parentExpression));
            ParentIdExpression = parentIdExpression ?? throw new ArgumentNullException(nameof(parentIdExpression));
        }

        public Expression<Func<TEntity, object>> ForeignKeyExpression { get; }

        public Expression<Func<TEntity, object>> ParentExpression { get; }

        public Expression<Func<object, object>> ParentIdExpression { get; }
    }
}
