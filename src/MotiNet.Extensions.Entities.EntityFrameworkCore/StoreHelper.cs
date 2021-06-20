using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections;
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
        public static Expression<Func<T, TProperty>> BuildPropertyLambda<T, TProperty>(Expression<Func<T, TProperty>> propertyExpression)
            => BuildPropertyLambda<T, TProperty>(propertyExpression.GetPropertyAccess().Name);

        public static Expression<Func<T, TProperty>> BuildPropertyLambda<T, TProperty>(string name)
        {
            var parameterExpression = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameterExpression, name);
            var lambda = Expression.Lambda<Func<T, TProperty>>(property, parameterExpression);

            return lambda;
        }

        public static Expression<Func<T, bool>> BuildPropertyLambda<T>(Expression<Func<T, object>> propertyExpression, object value)
            => BuildPropertyLambda<T>(propertyExpression.GetPropertyAccess().Name, value);

        public static Expression<Func<T, bool>> BuildPropertyLambda<T>(string name, object value)
        {
            var parameterExpression = Expression.Parameter(typeof(T), "x");
            var constant = Expression.Constant(value);
            var property = Expression.Property(parameterExpression, name);
            var equalsExpression = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equalsExpression, parameterExpression);

            return lambda;
        }

        public static Expression<Func<T, bool>> BuildPropertyLambda<T>(Expression<Func<T, object>> propertyExpression, IList values)
            => BuildPropertyLambda<T>(propertyExpression.GetPropertyAccess().Name, values);

        public static Expression<Func<T, bool>> BuildPropertyLambda<T>(string name, IList values)
        {
            var parameterExpression = Expression.Parameter(typeof(T), "x");
            var constant = Expression.Constant(values);
            var property = Expression.Property(parameterExpression, name);
            var containsMethod = values.GetType().GetMethod(nameof(Enumerable.Contains));
            var containsExpression = Expression.Call(constant, containsMethod, property);
            var lambda = Expression.Lambda<Func<T, bool>>(containsExpression, parameterExpression);

            return lambda;
        }

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
                var keyType = thisId.GetType();

                var otherIdsQuery = BuildManyToManyRelationshipOtherIdsQuery(keyType, thisId, dbContext, include);
                var otherIds = QueryToList(otherIdsQuery, keyType);
                var othersQuery = (IQueryable<object>)BuildManyToManyRelationshipOthersQuery(otherIds, dbContext, include);

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
                var keyType = thisId.GetType();

                var otherIdsQuery = BuildManyToManyRelationshipOtherIdsQuery(keyType, thisId, dbContext, include);
                var otherIds = await QueryToListAsync(otherIdsQuery, keyType, cancellationToken);
                var othersQuery = (IQueryable<object>)BuildManyToManyRelationshipOthersQuery(otherIds, dbContext, include);

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
                        var linkConstructor = relationship.LinkType.GetConstructor(Array.Empty<Type>());
                        foreach (var newOtherId in newOtherIds)
                        {
                            var link = linkConstructor.Invoke(Array.Empty<object>());
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
                        var linkConstructor = relationship.LinkType.GetConstructor(Array.Empty<Type>());
                        foreach (var newOtherId in newOtherIds)
                        {
                            var link = linkConstructor.Invoke(Array.Empty<object>());
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
                        var keyType = thisId.GetType();
                        var oldOtherIdsQuery = BuildManyToManyRelationshipOtherIdsQuery(keyType, thisId, dbContext, relationship);
                        var oldOtherIds = QueryToList(oldOtherIdsQuery, keyType);

                        var linkConstructor = relationship.LinkType.GetConstructor(Array.Empty<Type>());

                        foreach (var newOtherId in newOtherIds)
                        {
                            if (!oldOtherIds.Contains(newOtherId))
                            {
                                var link = linkConstructor.Invoke(Array.Empty<object>());
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
                                var link = linkConstructor.Invoke(Array.Empty<object>());
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
                        var keyType = thisId.GetType();
                        var oldOtherIdsQuery = BuildManyToManyRelationshipOtherIdsQuery(keyType, thisId, dbContext, relationship);
                        var oldOtherIds = await QueryToListAsync(oldOtherIdsQuery, keyType, cancellationToken);

                        var linkConstructor = relationship.LinkType.GetConstructor(Array.Empty<Type>());

                        foreach (var newOtherId in newOtherIds)
                        {
                            if (!oldOtherIds.Contains(newOtherId))
                            {
                                var link = linkConstructor.Invoke(Array.Empty<object>());
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
                                var link = linkConstructor.Invoke(Array.Empty<object>());
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

        #region Helpers

        private static IList QueryToList(IQueryable query, Type type)
        {
            // var list = query.ToList();
            var toListMethod = typeof(Enumerable)
                .GetMethod((nameof(Enumerable.ToList)))
                .MakeGenericMethod(type);
            var list = (IList)toListMethod.Invoke(null, new object[] { query });

            return list;
        }

        private static async Task<IList> QueryToListAsync(IQueryable query, Type type, CancellationToken cancellationToken)
        {
            // var list = await query.ToListAsync(cancellationToken);
            var toListMethod = typeof(EntityFrameworkQueryableExtensions)
                .GetMethod((nameof(EntityFrameworkQueryableExtensions.ToListAsync)))
                .MakeGenericMethod(type);
            var listTask = (Task)toListMethod.Invoke(null, new object[] { query, cancellationToken });
            await listTask;
            var resultProperty = typeof(Task<>).MakeGenericType(typeof(List<>).MakeGenericType(type))
                .GetProperty(nameof(Task<object>.Result));
            var list = (IList)resultProperty.GetValue(listTask);

            return list;
        }

        private static IQueryable BuildManyToManyRelationshipOtherIdsQuery<T>(
            Type keyType, object thisId, DbContext dbContext,
            ManyToManyIncludeSpecification<T> manyToManyInclude)
            where T : class
            => BuildManyToManyRelationshipOtherIdsQuery<T>(
                keyType, thisId, dbContext,
                manyToManyInclude.LinkType,
                manyToManyInclude.LinkForeignKeyToThisExpression,
                manyToManyInclude.LinkForeignKeyToOtherExpression);

        private static IQueryable BuildManyToManyRelationshipOtherIdsQuery<T>(
            Type keyType, object thisId, DbContext dbContext,
            ManyToManyRelationshipSpecification<T> manyToManyRelationship)
            where T : class
            => BuildManyToManyRelationshipOtherIdsQuery<T>(
                keyType, thisId, dbContext,
                manyToManyRelationship.LinkType,
                manyToManyRelationship.LinkForeignKeyToThisExpression,
                manyToManyRelationship.LinkForeignKeyToOtherExpression);

        private static IQueryable BuildManyToManyRelationshipOtherIdsQuery<T>(
            Type keyType, object thisId, DbContext dbContext,
            Type linkType,
            Expression<Func<object, object>> linkForeignKeyToThisExpression,
            Expression<Func<object, object>> linkForeignKeyToOtherExpression)
            where T : class
        {
            // var linkSet = dbContext.Set<LinkEntity>()
            var linkSetMethod = dbContext.GetType().GetMethod(nameof(dbContext.Set), Array.Empty<Type>()).MakeGenericMethod(linkType);
            var linkSet = linkSetMethod.Invoke(dbContext, Array.Empty<object>());

            // linksQuery = linkSet.Where(x => x.ThisEntityId = entity.Id)
            var buildPropertyLambdaMethod = typeof(StoreHelper)
                .GetMethod(nameof(BuildPropertyLambda), new Type[] { typeof(string), typeof(object) })
                .MakeGenericMethod(linkType);
            var lambda = buildPropertyLambdaMethod.Invoke(
                null,
                new object[] { linkForeignKeyToThisExpression.GetPropertyAccess().Name, thisId });
            var whereMethod = typeof(Queryable)
                .GetMethods()
                .Single(x => x.Name == nameof(Queryable.Where) &&
                             x.GetParameters().Last().ParameterType.GenericTypeArguments.First().GenericTypeArguments.Length == 2)
                .MakeGenericMethod(linkType);
            var linksQuery = whereMethod.Invoke(null, new object[] { linkSet, lambda });

            // var otherIdsQuery = linksQuery.Select(x => x.OtherEntityId)
            buildPropertyLambdaMethod = typeof(StoreHelper)
                .GetMethod(nameof(BuildPropertyLambda), new Type[] { typeof(string) })
                .MakeGenericMethod(linkType, keyType);
            lambda = buildPropertyLambdaMethod.Invoke(
                null,
                new object[] { linkForeignKeyToOtherExpression.GetPropertyAccess().Name });
            var selectMethod = typeof(Queryable)
                .GetMethods()
                .Single(x => x.Name == nameof(Queryable.Select) &&
                             x.GetParameters().Last().ParameterType.GenericTypeArguments.First().GenericTypeArguments.Length == 2)
                .MakeGenericMethod(linkType, keyType);
            var otherIdsQuery = (IQueryable)selectMethod.Invoke(null, new object[] { linksQuery, lambda });

            return otherIdsQuery;
        }

        private static IQueryable BuildManyToManyRelationshipOthersQuery<T>(IList otherIds, DbContext dbContext,
            ManyToManyIncludeSpecification<T> manyToManyInclude)
            where T : class
        {
            // var otherSet = dbContext.Set<OtherEntity>()
            var otherSetMethod = dbContext.GetType().GetMethod(nameof(dbContext.Set), Array.Empty<Type>()).MakeGenericMethod(manyToManyInclude.OtherType);
            var otherSet = otherSetMethod.Invoke(dbContext, Array.Empty<object>());

            // othersQuery = otherSet.Where(x => otherIds.Contains(x.Id))
            var buildPropertyLambdaMethod = typeof(StoreHelper)
                .GetMethod(nameof(BuildPropertyLambda), new Type[] { typeof(string), typeof(IList) })
                .MakeGenericMethod(manyToManyInclude.OtherType);
            var lambda = buildPropertyLambdaMethod.Invoke(
                null,
                new object[] { manyToManyInclude.OtherIdExpression.GetPropertyAccess().Name, otherIds });
            var whereMethod = typeof(Queryable)
                .GetMethods()
                .Single(x => x.Name == nameof(Queryable.Where) &&
                             x.GetParameters().Last().ParameterType.GenericTypeArguments.First().GenericTypeArguments.Length == 2)
                .MakeGenericMethod(manyToManyInclude.OtherType);
            var othersQuery = (IQueryable)whereMethod.Invoke(null, new object[] { otherSet, lambda });

            return othersQuery;
        }

        #endregion
    }
}
