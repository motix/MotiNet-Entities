using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public abstract class ModifySpecificationBase<TEntity, TKey> : IModifySpecification<TEntity, TKey>
        where TEntity : class
        where TKey : IEquatable<TKey>
    {
        public virtual ICollection<OneToManyRelationshipSpecification<TEntity, TKey>> OneToManyRelationships { get; }
            = new List<OneToManyRelationshipSpecification<TEntity, TKey>>();

        public virtual ICollection<ManyToManyRelationshipSpecification<TEntity, TKey>> ManyToManyRelationships { get; }
            = new List<ManyToManyRelationshipSpecification<TEntity, TKey>>();

        public void AddOneToManyRelationship(
            Expression<Func<TEntity, TKey>> foreignKeyExpression,
            Expression<Func<TEntity, object>> parentExpression,
            Expression<Func<object, TKey>> parentIdExpression)
        {
            OneToManyRelationships.Add(new OneToManyRelationshipSpecification<TEntity, TKey>(
                foreignKeyExpression, parentExpression, parentIdExpression));
        }

        public void AddManyToManyRelationship(
            Expression<Func<TEntity, TKey>> thisIdExpression,
            Expression<Func<object, TKey>> otherIdExpression,
            Expression<Func<TEntity, IEnumerable<object>>> othersExpression,
            Type linkType,
            Expression<Func<object, TKey>> linkForeignKeyToThisExpression,
            Expression<Func<object, TKey>> linkForeignKeyToOtherExpression)
        {
            ManyToManyRelationships.Add(new ManyToManyRelationshipSpecification<TEntity, TKey>(
                thisIdExpression, otherIdExpression, othersExpression, linkType, linkForeignKeyToThisExpression, linkForeignKeyToOtherExpression));
        }
    }
}
