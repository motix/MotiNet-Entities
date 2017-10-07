using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public class ManyToManyRelationshipSpecification<TEntity, TKey>
        where TEntity : class
        where TKey : IEquatable<TKey>
    {
        public ManyToManyRelationshipSpecification(
            Expression<Func<TEntity, TKey>> thisIdExpression,
            Expression<Func<object, TKey>> otherIdExpression,
            Expression<Func<TEntity, IEnumerable<object>>> othersExpression, 
            Type linkType, Expression<Func<object, TKey>> linkForeignKeyToThisExpression,
            Expression<Func<object, TKey>> linkForeignKeyToOtherExpression)
        {
            ThisIdExpression = thisIdExpression ?? throw new ArgumentNullException(nameof(thisIdExpression));
            OtherIdExpression = otherIdExpression ?? throw new ArgumentNullException(nameof(otherIdExpression));
            OthersExpression = othersExpression ?? throw new ArgumentNullException(nameof(othersExpression));
            LinkType = linkType ?? throw new ArgumentNullException(nameof(linkType));
            LinkForeignKeyToThisExpression = linkForeignKeyToThisExpression ?? throw new ArgumentNullException(nameof(linkForeignKeyToThisExpression));
            LinkForeignKeyToOtherExpression = linkForeignKeyToOtherExpression ?? throw new ArgumentNullException(nameof(linkForeignKeyToOtherExpression));
        }

        public Expression<Func<TEntity, TKey>> ThisIdExpression { get; }

        public Expression<Func<object, TKey>> OtherIdExpression { get; }

        public Expression<Func<TEntity, IEnumerable<object>>> OthersExpression { get; }

        public Type LinkType { get; }

        public Expression<Func<object, TKey>> LinkForeignKeyToThisExpression { get; }

        public Expression<Func<object, TKey>> LinkForeignKeyToOtherExpression { get; }
    }
}
