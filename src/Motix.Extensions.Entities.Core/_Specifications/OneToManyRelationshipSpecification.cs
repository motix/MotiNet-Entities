using System;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public class OneToManyRelationshipSpecification<TEntity, TKey>
        where TEntity : class
        where TKey : IEquatable<TKey>
    {
        public OneToManyRelationshipSpecification(
            Expression<Func<TEntity, TKey>> foreignKeyExpression,
            Expression<Func<TEntity, object>> parentExpression,
            Expression<Func<object, TKey>> parentIdExpression)
        {
            ForeignKeyExpression = foreignKeyExpression ?? throw new ArgumentNullException(nameof(foreignKeyExpression));
            ParentExpression = parentExpression ?? throw new ArgumentNullException(nameof(parentExpression));
            ParentIdExpression = parentIdExpression ?? throw new ArgumentNullException(nameof(parentIdExpression));
        }

        public Expression<Func<TEntity, TKey>> ForeignKeyExpression { get; }

        public Expression<Func<TEntity, object>> ParentExpression { get; }

        public Expression<Func<object, TKey>> ParentIdExpression { get; }
    }
}
