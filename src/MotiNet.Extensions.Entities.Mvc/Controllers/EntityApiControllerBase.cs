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

        protected TEntityManager EntityManager { get; }

        protected virtual Expression<Func<TEntity, object>> EntityIdExpression => null;

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TEntityViewModel>>> Get()
        {
            var spec = new SearchSpecification<TEntity>(x => true);
            EntitiesSpecificationAction(spec);

            var models = await EntityManager.SearchAsync(spec);
            models = SortEntities(models);

            var viewModels = Mapper.Map<List<TEntityViewModel>>(models);
            await ProcessViewModels(viewModels, models);

            return viewModels;
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TEntityViewModel>> Get(TKey id)
        {
            TEntity model;

            if (EntityIdExpression == null)
            {
                model = await EntityManager.FindByIdAsync(id);
            }
            else
            {
                var spec = new FindSpecification<TEntity>(EntityIdExpression);
                EntitySpecificationAction(spec);
                model = await EntityManager.FindAsync(id, spec);
            }

            if (model == null)
            {
                return NotFound();
            }

            var viewModel = Mapper.Map<TEntityViewModel>(model);
            await ProcessViewModel(viewModel, model);

            return viewModel;
        }

        [HttpPost]
        public virtual async Task<ActionResult<TEntityViewModel>> Post(TEntityViewModel viewModel)
        {
            var model = Mapper.Map<TEntity>(viewModel);
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
            var model = Mapper.Map<TEntity>(viewModel);

            if (!Equals(id, (TKey)EntityManager.EntityAccessor.GetId(model)))
            {
                return BadRequest();
            }

            var result = await EntityManager.UpdateAsync(model);

            if (!result.Succeeded)
            {
                if (!await EntityExists(id))
                {
                    return NotFound();
                }

                return BadRequest(result);
            }

            return Mapper.Map<TEntityViewModel>(model);
        }

        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<TEntityViewModel>> Delete(TKey id)
        {
            var model = await EntityManager.FindByIdAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            var result = await EntityManager.DeleteAsync(model);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Mapper.Map<TEntityViewModel>(model);
        }

        protected virtual void EntitySpecificationAction(IFindSpecification<TEntity> specification) { }

        protected virtual void EntitiesSpecificationAction(ISearchSpecification<TEntity> specification) { }

        protected virtual IEnumerable<TEntity> SortEntities(IEnumerable<TEntity> entities) => entities;

        protected virtual Task ProcessViewModel(TEntityViewModel viewModel, TEntity model) => Task.FromResult(0);

        protected virtual async Task ProcessViewModels(IEnumerable<TEntityViewModel> viewModels, IEnumerable<TEntity> models)
        {
            for (var i = 0; i < viewModels.Count(); i++)
            {
                var viewModel = viewModels.ElementAt(i);
                var model = models.ElementAt(i);
                await ProcessViewModel(viewModel, model);
            }
        }

        protected virtual async Task<bool> EntityExists(TKey id)
        {
            var model = await EntityManager.FindByIdAsync(id);
            return model != null;
        }
    }
}
