using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MotiNet.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MotiNet.Extensions.Entities.Mvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class EntityApiControllerBase<TKey, TEntity, TEntityViewModel, TEntityManager> : ControllerBase
        where TKey : IEquatable<TKey>
        where TEntity : class
        where TEntityViewModel : class
        where TEntityManager : class, IEntityManager<TEntity>
    {
        public EntityApiControllerBase(TEntityManager entityManager)
        {
            EntityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
        }

        protected virtual bool IgnoreDeleteMark => false;

        protected virtual bool IsDeleteMarkEntity => EntityManager is IDeleteMarkEntityManager<TEntity> && !IgnoreDeleteMark;

        protected TEntityManager EntityManager { get; }

        protected IDeleteMarkEntityManager<TEntity> DeleteMarkEntityManager => EntityManager as IDeleteMarkEntityManager<TEntity>;

        protected virtual Expression<Func<TEntity, object>> EntityIdExpression => null;

        protected virtual Expression<Func<TEntity, bool>> EntityNotDeleteMarkedExpression => null;

        [HttpGet]
        public virtual Task<ActionResult<IEnumerable<TEntityViewModel>>> Get() => Get(x => true);

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TEntityViewModel>> Get(TKey id)
        {
            TEntity model;

            if (EntityIdExpression == null)
            {
                model = await FindByIdAsync(id);
            }
            else
            {
                var spec = new FindSpecification<TEntity>(EntityIdExpression, IsDeleteMarkEntity ? EntityNotDeleteMarkedExpression : null);
                EntitySpecificationAction(spec);
                model = await EntityManager.FindAsync(id, spec);
            }

            if (model == null)
            {
                return NotFound();
            }

            var viewModel = Mapper.Map<TEntityViewModel>(model);
            ProcessViewModelForGet(viewModel, model);

            return viewModel;
        }

        [HttpPost]
        public virtual async Task<ActionResult<TEntityViewModel>> Post(TEntityViewModel viewModel)
        {
            var model = Mapper.Map<TEntity>(viewModel);
            ProcessModelForCreate(model);
            var result = await EntityManager.CreateAsync(model);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            viewModel = Mapper.Map<TEntityViewModel>(model);
            return CreatedAtAction(nameof(Get), new { id = EntityManager.EntityAccessor.GetId(model) }, viewModel);
        }

        [HttpPut("{id}")]
        public virtual async Task<ActionResult<TEntityViewModel>> Put(TKey id, TEntityViewModel viewModel)
        {
            var oldModel = await FindByIdAsync(id);

            if (oldModel == null)
            {
                return NotFound();
            }

            var model = Mapper.Map<TEntity>(viewModel);

            if (!Equals(id, (TKey)EntityManager.EntityAccessor.GetId(model)))
            {
                return BadRequest();
            }

            ProcessModelForUpdate(model);
            var result = await EntityManager.UpdateAsync(model);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Mapper.Map<TEntityViewModel>(model);
        }

        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<TEntityViewModel>> Delete(TKey id)
        {
            var model = await FindByIdAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            GenericResult result;

            if (IsDeleteMarkEntity)
            {
                result = await DeleteMarkEntityManager.MarkDeletedAsync(model, x => ProcessModelForMarkDeleted(x));
            }
            else
            {
                result = await EntityManager.DeleteAsync(model);
            }

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Mapper.Map<TEntityViewModel>(model);
        }

        protected virtual async Task<ActionResult<IEnumerable<TEntityViewModel>>> Get(Expression<Func<TEntity, bool>> criteria)
        {
            if (IsDeleteMarkEntity)
            {
                criteria = criteria.And(EntityNotDeleteMarkedExpression);
            }

            var spec = new SearchSpecification<TEntity>(criteria);
            EntitiesSpecificationAction(spec);

            var models = await EntityManager.SearchAsync(spec);

            return Get(models);
        }

        protected virtual ActionResult<IEnumerable<TEntityViewModel>> Get(IEnumerable<TEntity> models)
        {
            models = SortEntities(models);

            var viewModels = Mapper.Map<List<TEntityViewModel>>(models);
            ProcessViewModelsForGet(viewModels, models);

            return viewModels;
        }

        protected virtual void EntitySpecificationAction(IFindSpecification<TEntity> specification) { }

        protected virtual void EntitiesSpecificationAction(ISearchSpecification<TEntity> specification) { }

        protected virtual IEnumerable<TEntity> SortEntities(IEnumerable<TEntity> entities) => entities;

        protected virtual void ProcessViewModelForGet(TEntityViewModel viewModel, TEntity model) { }

        protected virtual void ProcessViewModelsForGet(IEnumerable<TEntityViewModel> viewModels, IEnumerable<TEntity> models)
        {
            for (var i = 0; i < viewModels.Count(); i++)
            {
                var viewModel = viewModels.ElementAt(i);
                var model = models.ElementAt(i);
                ProcessViewModelForGet(viewModel, model);
            }
        }

        protected virtual void ProcessModelForCreate(TEntity model) { }

        protected virtual void ProcessModelForUpdate(TEntity model) { }

        protected virtual void ProcessModelForMarkDeleted(TEntity model) { }

        protected virtual async Task<TEntity> FindByIdAsync(TKey id)
        {
            var model = await EntityManager.FindByIdAsync(id);
            if (IsDeleteMarkEntity && model != null && DeleteMarkEntityManager.DeleteMarkEntityAccessor.GetDeleteMarked(model))
            {
                model = null;
            }

            return model;
        }
    }
}
