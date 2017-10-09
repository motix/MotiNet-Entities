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
    public class EntityFrameworkRepository<TEntity, TDbContext> : IRepository<TEntity>, IAsyncRepository<TEntity>
        where TEntity : class
        where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;

        public EntityFrameworkRepository(TDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        #region Public Operations

        #region FindById

        public virtual TEntity FindById(object id)
        {
            ThrowIfDisposed();
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return _dbContext.Set<TEntity>().Find(new object[] { id });
        }

        public virtual Task<TEntity> FindByIdAsync(object id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return _dbContext.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
        }

        public virtual TEntity FindById(object id, IFindSpecification<TEntity> spec)
        {
            ThrowIfDisposed();
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var entities = _dbContext.Set<TEntity>().AsQueryable();

            entities = Include(entities, spec);

            var result = entities.SingleOrDefault(x => Equals(GetPropertyValue(x, spec.KeyExpression), id));

            if (spec.ManyToManyIncludes != null)
            {
                FillManyToManyRelationships(result, spec.ManyToManyIncludes);
            }

            return result;
        }

        public virtual async Task<TEntity> FindByIdAsync(object id, IFindSpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var entities = _dbContext.Set<TEntity>().AsQueryable();

            entities = Include(entities, spec);

            var result = await entities.SingleOrDefaultAsync(x => Equals(GetPropertyValue(x, spec.KeyExpression), id), cancellationToken);

            if (spec.ManyToManyIncludes != null)
            {
                await FillManyToManyRelationshipsAsync(result, spec.ManyToManyIncludes, cancellationToken);
            }

            return result;
        }

        #endregion

        #region All

        public virtual IEnumerable<TEntity> All()
        {
            ThrowIfDisposed();

            return _dbContext.Set<TEntity>().ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> AllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return await _dbContext.Set<TEntity>().ToListAsync(cancellationToken);
        }

        #endregion

        #region Search

        public virtual IEnumerable<TEntity> Search(ISearchSpecification<TEntity> spec)
        {
            ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var entities = _dbContext.Set<TEntity>().AsQueryable();

            entities = Include(entities, spec);

            if (spec.Criteria != null)
            {
                entities = entities.Where(spec.Criteria);
            }

            var result = entities.ToList();

            if (spec.ManyToManyIncludes != null)
            {
                FillManyToManyRelationships(result, spec.ManyToManyIncludes);
            }

            return result;
        }

        public virtual async Task<IEnumerable<TEntity>> SearchAsync(ISearchSpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var entities = _dbContext.Set<TEntity>().AsQueryable();

            entities = Include(entities, spec);

            if (spec.Criteria != null)
            {
                entities = entities.Where(spec.Criteria);
            }

            var result = await entities.ToListAsync(cancellationToken);

            if (spec.ManyToManyIncludes != null)
            {
                await FillManyToManyRelationshipsAsync(result, spec.ManyToManyIncludes, cancellationToken);
            }

            return result;
        }

        public virtual PagedSearchResult<TEntity> Search(IPagedSearchSpecification<TEntity> spec)
        {
            ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var entities = _dbContext.Set<TEntity>().AsQueryable();

            entities = Include(entities, spec);

            if (spec.ScopeCriteria != null)
            {
                entities = entities.Where(spec.ScopeCriteria);
            }

            var totalCount = entities.Count();

            if (spec.Criteria != null)
            {
                entities = entities.Where(spec.Criteria);
            }

            var resultCount = entities.Count();

            entities = Order(entities, spec);

            entities = Page(entities, spec);

            var result = entities.ToList();

            if (spec.ManyToManyIncludes != null)
            {
                FillManyToManyRelationships(result, spec.ManyToManyIncludes);
            }

            return new PagedSearchResult<TEntity>(totalCount, resultCount, result);
        }

        public virtual async Task<PagedSearchResult<TEntity>> SearchAsync(IPagedSearchSpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var entities = _dbContext.Set<TEntity>().AsQueryable();

            entities = Include(entities, spec);

            if (spec.ScopeCriteria != null)
            {
                entities = entities.Where(spec.ScopeCriteria);
            }

            var totalCount = await entities.CountAsync(cancellationToken);

            if (spec.Criteria != null)
            {
                entities = entities.Where(spec.Criteria);
            }

            var resultCount = await entities.CountAsync(cancellationToken);

            entities = Order(entities, spec);

            entities = Page(entities, spec);

            var result = await entities.ToListAsync(cancellationToken);

            if (spec.ManyToManyIncludes != null)
            {
                await FillManyToManyRelationshipsAsync(result, spec.ManyToManyIncludes, cancellationToken);
            }

            return new PagedSearchResult<TEntity>(totalCount, resultCount, result);
        }

        #endregion

        #region Add

        public virtual TEntity Add(TEntity entity)
        {
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbContext.Set<TEntity>().Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbContext.Set<TEntity>().Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public virtual TEntity Add(TEntity entity, IModifySpecification<TEntity> spec)
        {
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            PrepareOneToManyRelationships(entity, spec);

            _dbContext.Set<TEntity>().Add(entity);
            _dbContext.SaveChanges();

            AddManyToManyRelationships(entity, spec);

            return entity;
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity, IModifySpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            PrepareOneToManyRelationships(entity, spec);

            _dbContext.Set<TEntity>().Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await AddManyToManyRelationshipsAsync(entity, spec, cancellationToken);

            return entity;
        }

        #endregion

        #region Update

        public virtual void Update(TEntity entity)
        {
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual void Update(TEntity entity, IModifySpecification<TEntity> spec)
        {
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            PrepareOneToManyRelationships(entity, spec);

            UpdateManyToManyRelationships(entity, spec);

            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public virtual async Task UpdateAsync(TEntity entity, IModifySpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            PrepareOneToManyRelationships(entity, spec);

            await UpdateManyToManyRelationshipsAsync(entity, spec, cancellationToken);

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region Delete

        public virtual void Delete(TEntity entity)
        {
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbContext.Set<TEntity>().Remove(entity);
            _dbContext.SaveChanges();
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #endregion

        #region Helpers

        private TResult GetPropertyValue<T, TResult>(T obj, Expression<Func<T, TResult>> expression)
        {
            var body = expression.Body;
            MemberExpression propertySelector = body as MemberExpression ?? (MemberExpression)((UnaryExpression)body).Operand;
            var property = (PropertyInfo)propertySelector.Member;
            return (TResult)property.GetValue(obj);
        }

        private void SetPropertyValue<T, TValue>(T obj, Expression<Func<T, TValue>> expression, TValue value)
        {
            var body = expression.Body;
            MemberExpression propertySelector = body as MemberExpression ?? (MemberExpression)((UnaryExpression)body).Operand;
            var property = (PropertyInfo)propertySelector.Member;
            property.SetValue(obj, value);
        }

        private IQueryable<TEntity> Include(IQueryable<TEntity> entities, IGetSpecification<TEntity> spec)
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

        private void FillManyToManyRelationships<T>(T entity,
            IEnumerable<ManyToManyIncludeSpecification<T>> manyToManyIncludes)
            where T : class
        {
            foreach (var include in manyToManyIncludes)
            {
                var thisId = GetPropertyValue(entity, include.ThisIdExpression);
                var linkSetMethod = _dbContext.GetType().GetMethod(nameof(_dbContext.Set)).MakeGenericMethod(include.LinkType);
                var linkSet = (IQueryable<object>)linkSetMethod.Invoke(_dbContext, new object[0]);
                var links = linkSet.Where(x => Equals(GetPropertyValue(x, include.LinkForeignKeyToThisExpression), thisId));
                var otherIds = links.Select(x => GetPropertyValue(x, include.LinkForeignKeyToOtherExpression)).ToList();
                var otherSetMethod = _dbContext.GetType().GetMethod(nameof(_dbContext.Set)).MakeGenericMethod(include.OtherType);
                var otherSet = (IQueryable<object>)otherSetMethod.Invoke(_dbContext, new object[0]);
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
                    FillManyToManyRelationships((IEnumerable<object>)otherList, include.ChildManyToManyIncludes);
                }
            }
        }

        private async Task FillManyToManyRelationshipsAsync<T>(T entity,
            IEnumerable<ManyToManyIncludeSpecification<T>> manyToManyIncludes, CancellationToken cancellationToken)
            where T : class
        {
            foreach (var include in manyToManyIncludes)
            {
                var thisId = GetPropertyValue(entity, include.ThisIdExpression);
                var linkSetMethod = _dbContext.GetType().GetMethod(nameof(_dbContext.Set)).MakeGenericMethod(include.LinkType);
                var linkSet = (IQueryable<object>)linkSetMethod.Invoke(_dbContext, new object[0]);
                var links = linkSet.Where(x => Equals(GetPropertyValue(x, include.LinkForeignKeyToThisExpression), thisId));
                var otherIds = await links.Select(x => GetPropertyValue(x, include.LinkForeignKeyToOtherExpression)).ToListAsync(cancellationToken);
                var otherSetMethod = _dbContext.GetType().GetMethod(nameof(_dbContext.Set)).MakeGenericMethod(include.OtherType);
                var otherSet = (IQueryable<object>)otherSetMethod.Invoke(_dbContext, new object[0]);
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
                    await FillManyToManyRelationshipsAsync((IEnumerable<object>)otherList, include.ChildManyToManyIncludes, cancellationToken);
                }
            }
        }

        private void FillManyToManyRelationships<T>(IEnumerable<T> entities,
            IEnumerable<ManyToManyIncludeSpecification<T>> manyToManyIncludes)
            where T : class
        {
            foreach (var entity in entities)
            {
                FillManyToManyRelationships(entity, manyToManyIncludes);
            }
        }

        private async Task FillManyToManyRelationshipsAsync<T>(IEnumerable<T> entities,
            IEnumerable<ManyToManyIncludeSpecification<T>> manyToManyIncludes, CancellationToken cancellationToken)
            where T : class
        {
            foreach (var entity in entities)
            {
                await FillManyToManyRelationshipsAsync(entity, manyToManyIncludes, cancellationToken);
            }
        }

        private IQueryable<TEntity> Order(IQueryable<TEntity> entities, IPagedSearchSpecification<TEntity> spec)
        {
            if (spec.Orders != null)
            {
                entities = spec.Orders.Aggregate(entities, (current, order) =>
                {
                    if (current is IOrderedQueryable<TEntity>)
                    {
                        if (order.IsDescending)
                        {
                            return ((IOrderedQueryable<TEntity>)current).ThenByDescending(order.OrderExpression);
                        }

                        return ((IOrderedQueryable<TEntity>)current).ThenBy(order.OrderExpression);
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

        private IQueryable<TEntity> Page(IQueryable<TEntity> entities, IPagedSearchSpecification<TEntity> spec)
        {
            if (spec.PageSize.HasValue)
            {
                var offset = ((spec.PageNumber - 1) * spec.PageSize) ?? 0;
                entities = entities.Skip(offset).Take(spec.PageSize.Value);
            }

            return entities;
        }

        private void PrepareOneToManyRelationships(TEntity entity, IModifySpecification<TEntity> spec)
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

        private void AddManyToManyRelationships(TEntity entity, IModifySpecification<TEntity> spec)
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
                            _dbContext.Add(link);
                        }
                        _dbContext.SaveChanges();
                    }
                }
            }
        }

        private async Task AddManyToManyRelationshipsAsync(TEntity entity, IModifySpecification<TEntity> spec, CancellationToken cancellationToken)
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
                            _dbContext.Add(link);
                        }
                        await _dbContext.SaveChangesAsync(cancellationToken);
                    }
                }
            }
        }

        private void UpdateManyToManyRelationships(TEntity entity, IModifySpecification<TEntity> spec)
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
                        var linkSetMethod = _dbContext.GetType().GetMethod(nameof(_dbContext.Set)).MakeGenericMethod(relationship.LinkType);
                        var linkSet = (IQueryable<object>)linkSetMethod.Invoke(_dbContext, new object[0]);
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
                                _dbContext.Add(link);
                            }
                        }
                        _dbContext.SaveChanges();

                        foreach (var oldOtherId in oldOtherIds)
                        {
                            if (!newOtherIds.Contains(oldOtherId))
                            {
                                var link = linkConstructor.Invoke(new object[0]);
                                SetPropertyValue(link, relationship.LinkForeignKeyToThisExpression, thisId);
                                SetPropertyValue(link, relationship.LinkForeignKeyToOtherExpression, oldOtherId);
                                _dbContext.Remove(link);
                            }
                        }
                        _dbContext.SaveChanges();
                    }
                }
            }
        }

        private async Task UpdateManyToManyRelationshipsAsync(TEntity entity, IModifySpecification<TEntity> spec, CancellationToken cancellationToken)
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
                        var linkSetMethod = _dbContext.GetType().GetMethod(nameof(_dbContext.Set)).MakeGenericMethod(relationship.LinkType);
                        var linkSet = (IQueryable<object>)linkSetMethod.Invoke(_dbContext, new object[0]);
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
                                _dbContext.Add(link);
                            }
                        }
                        await _dbContext.SaveChangesAsync(cancellationToken);

                        foreach (var oldOtherId in oldOtherIds)
                        {
                            if (!newOtherIds.Contains(oldOtherId))
                            {
                                var link = linkConstructor.Invoke(new object[0]);
                                SetPropertyValue(link, relationship.LinkForeignKeyToThisExpression, thisId);
                                SetPropertyValue(link, relationship.LinkForeignKeyToOtherExpression, oldOtherId);
                                _dbContext.Remove(link);
                            }
                        }
                        await _dbContext.SaveChangesAsync(cancellationToken);
                    }
                }
            }
        }

        #endregion

        #region IDisposable Support

        private bool _disposed = false; // To detect redundant calls

        public void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _dbContext.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposed = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~EfRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
