using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public class ManyToManyIncludeSpecification<TEntity>
        where TEntity : class
    {
        public ManyToManyIncludeSpecification(
            Expression<Func<TEntity, object>> thisIdExpression,
            Type otherType,
            Expression<Func<object, object>> otherIdExpression,
            Expression<Func<TEntity, IEnumerable<object>>> othersExpression,
            Type linkType,
            Expression<Func<object, object>> linkForeignKeyToThisExpression,
            Expression<Func<object, object>> linkForeignKeyToOtherExpression)
        {
            ThisIdExpression = thisIdExpression ?? throw new ArgumentNullException(nameof(thisIdExpression));
            OtherType = otherType ?? throw new ArgumentNullException(nameof(otherType));
            OtherIdExpression = otherIdExpression ?? throw new ArgumentNullException(nameof(otherIdExpression));
            OthersExpression = othersExpression ?? throw new ArgumentNullException(nameof(othersExpression));
            LinkType = linkType ?? throw new ArgumentNullException(nameof(linkType));
            LinkForeignKeyToThisExpression = linkForeignKeyToThisExpression ?? throw new ArgumentNullException(nameof(linkForeignKeyToThisExpression));
            LinkForeignKeyToOtherExpression = linkForeignKeyToOtherExpression ?? throw new ArgumentNullException(nameof(linkForeignKeyToOtherExpression));
        }

        public Expression<Func<TEntity, object>> ThisIdExpression { get; }

        public Type OtherType { get; }

        public Expression<Func<object, object>> OtherIdExpression { get; }

        public Expression<Func<TEntity, IEnumerable<object>>> OthersExpression { get; }

        public Type LinkType { get; }

        public Expression<Func<object, object>> LinkForeignKeyToThisExpression { get; }

        public Expression<Func<object, object>> LinkForeignKeyToOtherExpression { get; }

        public ICollection<Expression<Func<object, object>>> ChildIncludes { get; set; }
            = new List<Expression<Func<object, object>>>();

        public ICollection<string> ChildIncludeStrings { get; set; }
            = new List<string>();

        public ICollection<ManyToManyIncludeSpecification<object>> ChildManyToManyIncludes { get; set; }
            = new List<ManyToManyIncludeSpecification<object>>();
    }
}
