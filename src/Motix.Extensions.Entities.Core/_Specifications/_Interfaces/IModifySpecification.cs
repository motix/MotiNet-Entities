using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public interface IModifySpecification<TEntity, TKey>
        where TEntity : class
        where TKey : IEquatable<TKey>
    {
        ICollection<OneToManyRelationshipSpecification<TEntity, TKey>> OneToManyRelationships { get; }

        ICollection<ManyToManyRelationshipSpecification<TEntity, TKey>> ManyToManyRelationships { get; }

        void AddOneToManyRelationship(
            Expression<Func<TEntity, TKey>> foreignKeyExpression,
            Expression<Func<TEntity, object>> parentExpression,
            Expression<Func<object, TKey>> parentIdExpression);

        void AddManyToManyRelationship(
            Expression<Func<TEntity, TKey>> thisIdExpression,
            Expression<Func<object, TKey>> otherIdExpression,
            Expression<Func<TEntity, IEnumerable<object>>> othersExpression,
            Type linkType,
            Expression<Func<object, TKey>> linkForeignKeyToThisExpression,
            Expression<Func<object, TKey>> linkForeignKeyToOtherExpression);
    }
}
