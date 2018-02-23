using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities.EntityFrameworkCore
{
    internal static class StoreHelper
    {
        public static TResult GetPropertyValue<T, TResult>(T obj, Expression<Func<T, TResult>> expression)
        {
            var body = expression.Body;
            MemberExpression propertySelector = body as MemberExpression ?? (MemberExpression)((UnaryExpression)body).Operand;
            var property = (PropertyInfo)propertySelector.Member;
            return (TResult)property.GetValue(obj);
        }

        public static void SetPropertyValue<T, TValue>(T obj, Expression<Func<T, TValue>> expression, TValue value)
        {
            var body = expression.Body;
            MemberExpression propertySelector = body as MemberExpression ?? (MemberExpression)((UnaryExpression)body).Operand;
            var property = (PropertyInfo)propertySelector.Member;
            property.SetValue(obj, value);
        }

        public static IQueryable<T> Include<T>(IQueryable<T> entities, IGetSpecification<T> spec)
            where T : class
        {
            if (spec.Includes != null)
            {
                entities = spec.Includes.Aggregate(entities, (current, include) => current.Include(include));
            }

            if (spec.IncludeStrings != null)
            {
                entities = spec.IncludeStrings.Aggregate(entities, (current, include) => current.Include(include));
            }

            return entities;
        }

        public static void FillManyToManyRelationships<T>(T entity, DbContext dbContext,
            IEnumerable<ManyToManyIncludeSpecification<T>> manyToManyIncludes)
            where T : class
        {
            foreach (var include in manyToManyIncludes)
            {
                var thisId = GetPropertyValue(entity, include.ThisIdExpression);
                var linkSetMethod = dbContext.GetType().GetMethod(nameof(dbContext.Set)).MakeGenericMethod(include.LinkType);
                var linkSet = (IQueryable<object>)linkSetMethod.Invoke(dbContext, new object[0]);
                var links = linkSet.Where(x => Equals(GetPropertyValue(x, include.LinkForeignKeyToThisExpression), thisId));
                var otherIds = links.Select(x => GetPropertyValue(x, include.LinkForeignKeyToOtherExpression)).ToList();
                var otherSetMethod = dbContext.GetType().GetMethod(nameof(dbContext.Set)).MakeGenericMethod(include.OtherType);
                var otherSet = (IQueryable<object>)otherSetMethod.Invoke(dbContext, new object[0]);
                var othersQuery = otherSet.Where(x => otherIds.Contains(GetPropertyValue(x, include.OtherIdExpression)));

                if (include.ChildIncludes != null)
                {
                    othersQuery = include.ChildIncludes.Aggregate(othersQuery, (current, childInclude) => current.Include(childInclude));
                }

                if (include.ChildIncludeStrings != null)
                {
                    othersQuery = include.ChildIncludeStrings.Aggregate(othersQuery, (current, includex) => current.Include(includex));
                }

                var others = othersQuery.ToList();
                var otherListType = typeof(List<>).MakeGenericType(include.OtherType);
                var otherList = Activator.CreateInstance(otherListType);
                var addMethod = otherListType.GetMethod(nameof(List<object>.Add));
                foreach (var other in others)
                {
                    addMethod.Invoke(otherList, new object[] { other });
                }
                SetPropertyValue(entity, include.OthersExpression, (IEnumerable<object>)otherList);

                if (include.ChildManyToManyIncludes != null)
                {
                    FillManyToManyRelationships((IEnumerable<object>)otherList, dbContext, include.ChildManyToManyIncludes);
                }
            }
        }

        public static async Task FillManyToManyRelationshipsAsync<T>(T entity, DbContext dbContext,
            IEnumerable<ManyToManyIncludeSpecification<T>> manyToManyIncludes, CancellationToken cancellationToken)
            where T : class
        {
            foreach (var include in manyToManyIncludes)
            {
                var thisId = GetPropertyValue(entity, include.ThisIdExpression);
                var linkSetMethod = dbContext.GetType().GetMethod(nameof(dbContext.Set)).MakeGenericMethod(include.LinkType);
                var linkSet = (IQueryable<object>)linkSetMethod.Invoke(dbContext, new object[0]);
                var links = linkSet.Where(x => Equals(GetPropertyValue(x, include.LinkForeignKeyToThisExpression), thisId));
                var otherIds = await links.Select(x => GetPropertyValue(x, include.LinkForeignKeyToOtherExpression)).ToListAsync(cancellationToken);
                var otherSetMethod = dbContext.GetType().GetMethod(nameof(dbContext.Set)).MakeGenericMethod(include.OtherType);
                var otherSet = (IQueryable<object>)otherSetMethod.Invoke(dbContext, new object[0]);
                var othersQuery = otherSet.Where(x => otherIds.Contains(GetPropertyValue(x, include.OtherIdExpression)));

                if (include.ChildIncludes != null)
                {
                    othersQuery = include.ChildIncludes.Aggregate(othersQuery, (current, childInclude) => current.Include(childInclude));
                }

                if (include.ChildIncludeStrings != null)
                {
                    othersQuery = include.ChildIncludeStrings.Aggregate(othersQuery, (current, childInclude) => current.Include(childInclude));
                }

                var others = await othersQuery.ToListAsync(cancellationToken);
                var otherListType = typeof(List<>).MakeGenericType(include.OtherType);
                var otherList = Activator.CreateInstance(otherListType);
                var addMethod = otherListType.GetMethod(nameof(List<object>.Add));
                foreach (var other in others)
                {
                    addMethod.Invoke(otherList, new object[] { other });
                }
                SetPropertyValue(entity, include.OthersExpression, (IEnumerable<object>)otherList);

                if (include.ChildManyToManyIncludes != null)
                {
                    await FillManyToManyRelationshipsAsync((IEnumerable<object>)otherList, dbContext, include.ChildManyToManyIncludes, cancellationToken);
                }
            }
        }

        public static void FillManyToManyRelationships<T>(IEnumerable<T> entities, DbContext dbContext,
            IEnumerable<ManyToManyIncludeSpecification<T>> manyToManyIncludes)
            where T : class
        {
            foreach (var entity in entities)
            {
                FillManyToManyRelationships(entity, dbContext, manyToManyIncludes);
            }
        }

        public static async Task FillManyToManyRelationshipsAsync<T>(IEnumerable<T> entities, DbContext dbContext,
            IEnumerable<ManyToManyIncludeSpecification<T>> manyToManyIncludes, CancellationToken cancellationToken)
            where T : class
        {
            foreach (var entity in entities)
            {
                await FillManyToManyRelationshipsAsync(entity, dbContext, manyToManyIncludes, cancellationToken);
            }
        }

        public static IQueryable<T> Order<T>(IQueryable<T> entities, IPagedSearchSpecification<T> spec)
            where T : class
        {
            if (spec.Orders != null)
            {
                entities = spec.Orders.Aggregate(entities, (current, order) =>
                {
                    if (current is IOrderedQueryable<T>)
                    {
                        if (order.IsDescending)
                        {
                            return ((IOrderedQueryable<T>)current).ThenByDescending(order.OrderExpression);
                        }

                        return ((IOrderedQueryable<T>)current).ThenBy(order.OrderExpression);
                    }

                    if (order.IsDescending)
                    {
                        return current.OrderByDescending(order.OrderExpression);
                    }
                    return current.OrderBy(order.OrderExpression);
                });
            }

            return entities;
        }

        public static IQueryable<T> Page<T>(IQueryable<T> entities, IPagedSearchSpecification<T> spec)
            where T : class
        {
            if (spec.PageSize.HasValue)
            {
                var offset = ((spec.PageNumber - 1) * spec.PageSize) ?? 0;
                entities = entities.Skip(offset).Take(spec.PageSize.Value);
            }

            return entities;
        }

        public static void PrepareOneToManyRelationships<T>(T entity, IModifySpecification<T> spec)
            where T : class
        {
            if (spec.OneToManyRelationships != null)
            {
                foreach (var relationship in spec.OneToManyRelationships)
                {
                    var foreignKey = GetPropertyValue(entity, relationship.ForeignKeyExpression);
                    bool updateFromParent;

                    if (foreignKey == null)
                    {
                        updateFromParent = true;
                    }
                    else
                    {
                        var type = foreignKey.GetType();
                        if (type.IsValueType)
                        {
                            var defaultValue = Activator.CreateInstance(type);
                            updateFromParent = Equals(foreignKey, defaultValue);
                        }
                        else
                        {
                            updateFromParent = false;
                        }
                    }

                    if (updateFromParent)
                    {
                        var parent = GetPropertyValue(entity, relationship.ParentExpression);

                        if (parent != null)
                        {
                            var parentId = GetPropertyValue(parent, relationship.ParentIdExpression);

                            SetPropertyValue(entity, relationship.ForeignKeyExpression, parentId);
                            SetPropertyValue(entity, relationship.ParentExpression, null);
                        }
                    }
                }
            }
        }

        public static void AddManyToManyRelationships<T>(T entity, DbContext dbContext, IModifySpecification<T> spec)
            where T : class
        {
            if (spec.ManyToManyRelationships != null)
            {
                foreach (var relationship in spec.ManyToManyRelationships)
                {
                    var others = GetPropertyValue(entity, relationship.OthersExpression);
                    if (others != null)
                    {
                        var newOtherIds = others.AsQueryable().Select(x => GetPropertyValue(x, relationship.OtherIdExpression)).ToList();
                        var thisId = GetPropertyValue(entity, relationship.ThisIdExpression);
                        var linkConstructor = relationship.LinkType.GetConstructor(new Type[0]);
                        foreach (var newOtherId in newOtherIds)
                        {
                            var link = linkConstructor.Invoke(new object[0]);
                            SetPropertyValue(link, relationship.LinkForeignKeyToThisExpression, thisId);
                            SetPropertyValue(link, relationship.LinkForeignKeyToOtherExpression, newOtherId);
                            dbContext.Add(link);
                        }
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        public static async Task AddManyToManyRelationshipsAsync<T>(T entity, DbContext dbContext, IModifySpecification<T> spec, CancellationToken cancellationToken)
            where T : class
        {
            if (spec.ManyToManyRelationships != null)
            {
                foreach (var relationship in spec.ManyToManyRelationships)
                {
                    var others = GetPropertyValue(entity, relationship.OthersExpression);
                    if (others != null)
                    {
                        var newOtherIds = others.AsQueryable().Select(x => GetPropertyValue(x, relationship.OtherIdExpression)).ToList();
                        var thisId = GetPropertyValue(entity, relationship.ThisIdExpression);
                        var linkConstructor = relationship.LinkType.GetConstructor(new Type[0]);
                        foreach (var newOtherId in newOtherIds)
                        {
                            var link = linkConstructor.Invoke(new object[0]);
                            SetPropertyValue(link, relationship.LinkForeignKeyToThisExpression, thisId);
                            SetPropertyValue(link, relationship.LinkForeignKeyToOtherExpression, newOtherId);
                            dbContext.Add(link);
                        }
                        await dbContext.SaveChangesAsync(cancellationToken);
                    }
                }
            }
        }

        public static void UpdateManyToManyRelationships<T>(T entity, DbContext dbContext, IModifySpecification<T> spec)
            where T : class
        {
            if (spec.ManyToManyRelationships != null)
            {
                foreach (var relationship in spec.ManyToManyRelationships)
                {
                    var others = GetPropertyValue(entity, relationship.OthersExpression);
                    if (others != null)
                    {
                        var newOtherIds = others.AsQueryable().Select(x => GetPropertyValue(x, relationship.OtherIdExpression)).ToList();
                        var thisId = GetPropertyValue(entity, relationship.ThisIdExpression);
                        var linkSetMethod = dbContext.GetType().GetMethod(nameof(dbContext.Set)).MakeGenericMethod(relationship.LinkType);
                        var linkSet = (IQueryable<object>)linkSetMethod.Invoke(dbContext, new object[0]);
                        var oldLinks = linkSet.Where(x => Equals(GetPropertyValue(x, relationship.LinkForeignKeyToThisExpression), thisId));
                        var oldOtherIds = oldLinks.Select(x => GetPropertyValue(x, relationship.LinkForeignKeyToOtherExpression)).ToList();
                        var linkConstructor = relationship.LinkType.GetConstructor(new Type[0]);

                        foreach (var newOtherId in newOtherIds)
                        {
                            if (!oldOtherIds.Contains(newOtherId))
                            {
                                var link = linkConstructor.Invoke(new object[0]);
                                SetPropertyValue(link, relationship.LinkForeignKeyToThisExpression, thisId);
                                SetPropertyValue(link, relationship.LinkForeignKeyToOtherExpression, newOtherId);
                                dbContext.Add(link);
                            }
                        }
                        dbContext.SaveChanges();

                        foreach (var oldOtherId in oldOtherIds)
                        {
                            if (!newOtherIds.Contains(oldOtherId))
                            {
                                var link = linkConstructor.Invoke(new object[0]);
                                SetPropertyValue(link, relationship.LinkForeignKeyToThisExpression, thisId);
                                SetPropertyValue(link, relationship.LinkForeignKeyToOtherExpression, oldOtherId);
                                dbContext.Remove(link);
                            }
                        }
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        public static async Task UpdateManyToManyRelationshipsAsync<T>(T entity, DbContext dbContext, IModifySpecification<T> spec, CancellationToken cancellationToken)
            where T : class
        {
            if (spec.ManyToManyRelationships != null)
            {
                foreach (var relationship in spec.ManyToManyRelationships)
                {
                    var others = GetPropertyValue(entity, relationship.OthersExpression);
                    if (others != null)
                    {
                        var newOtherIds = others.AsQueryable().Select(x => GetPropertyValue(x, relationship.OtherIdExpression)).ToList();
                        var thisId = GetPropertyValue(entity, relationship.ThisIdExpression);
                        var linkSetMethod = dbContext.GetType().GetMethod(nameof(dbContext.Set)).MakeGenericMethod(relationship.LinkType);
                        var linkSet = (IQueryable<object>)linkSetMethod.Invoke(dbContext, new object[0]);
                        var oldLinks = linkSet.Where(x => Equals(GetPropertyValue(x, relationship.LinkForeignKeyToThisExpression), thisId));
                        var oldOtherIds = await oldLinks.Select(x => GetPropertyValue(x, relationship.LinkForeignKeyToOtherExpression)).ToListAsync(cancellationToken);
                        var linkConstructor = relationship.LinkType.GetConstructor(new Type[0]);

                        foreach (var newOtherId in newOtherIds)
                        {
                            if (!oldOtherIds.Contains(newOtherId))
                            {
                                var link = linkConstructor.Invoke(new object[0]);
                                SetPropertyValue(link, relationship.LinkForeignKeyToThisExpression, thisId);
                                SetPropertyValue(link, relationship.LinkForeignKeyToOtherExpression, newOtherId);
                                dbContext.Add(link);
                            }
                        }
                        await dbContext.SaveChangesAsync(cancellationToken);

                        foreach (var oldOtherId in oldOtherIds)
                        {
                            if (!newOtherIds.Contains(oldOtherId))
                            {
                                var link = linkConstructor.Invoke(new object[0]);
                                SetPropertyValue(link, relationship.LinkForeignKeyToThisExpression, thisId);
                                SetPropertyValue(link, relationship.LinkForeignKeyToOtherExpression, oldOtherId);
                                dbContext.Remove(link);
                            }
                        }
                        await dbContext.SaveChangesAsync(cancellationToken);
                    }
                }
            }
        }
    }
}
