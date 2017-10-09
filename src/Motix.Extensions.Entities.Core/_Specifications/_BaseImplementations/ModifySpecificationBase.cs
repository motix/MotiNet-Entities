using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public abstract class ModifySpecificationBase<TEntity> : IModifySpecification<TEntity>
        where TEntity : class
    {
        public virtual ICollection<OneToManyRelationshipSpecification<TEntity>> OneToManyRelationships { get; }
            = new List<OneToManyRelationshipSpecification<TEntity>>();

        public virtual ICollection<ManyToManyRelationshipSpecification<TEntity>> ManyToManyRelationships { get; }
            = new List<ManyToManyRelationshipSpecification<TEntity>>();

        public void AddOneToManyRelationship(
            Expression<Func<TEntity, object>> foreignKeyExpression,
            Expression<Func<TEntity, object>> parentExpression,
            Expression<Func<object, object>> parentIdExpression)
        {
            OneToManyRelationships.Add(new OneToManyRelationshipSpecification<TEntity>(
                foreignKeyExpression, parentExpression, parentIdExpression));
        }

        public void AddManyToManyRelationship(
            Expression<Func<TEntity, object>> thisIdExpression,
            Expression<Func<object, object>> otherIdExpression,
            Expression<Func<TEntity, IEnumerable<object>>> othersExpression,
            Type linkType,
            Expression<Func<object, object>> linkForeignKeyToThisExpression,
            Expression<Func<object, object>> linkForeignKeyToOtherExpression)
        {
            ManyToManyRelationships.Add(new ManyToManyRelationshipSpecification<TEntity>(
                thisIdExpression, otherIdExpression, othersExpression, linkType, linkForeignKeyToThisExpression, linkForeignKeyToOtherExpression));
        }
    }
}
