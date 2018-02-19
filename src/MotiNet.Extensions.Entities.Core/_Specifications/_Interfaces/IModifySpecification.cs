using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public interface IModifySpecification<TEntity>
        where TEntity : class
    {
        ICollection<OneToManyRelationshipSpecification<TEntity>> OneToManyRelationships { get; }

        ICollection<ManyToManyRelationshipSpecification<TEntity>> ManyToManyRelationships { get; }

        void AddOneToManyRelationship(
            Expression<Func<TEntity, object>> foreignKeyExpression,
            Expression<Func<TEntity, object>> parentExpression,
            Expression<Func<object, object>> parentIdExpression);

        void AddManyToManyRelationship(
            Expression<Func<TEntity, object>> thisIdExpression,
            Expression<Func<object, object>> otherIdExpression,
            Expression<Func<TEntity, IEnumerable<object>>> othersExpression,
            Type linkType,
            Expression<Func<object, object>> linkForeignKeyToThisExpression,
            Expression<Func<object, object>> linkForeignKeyToOtherExpression);
    }
}
