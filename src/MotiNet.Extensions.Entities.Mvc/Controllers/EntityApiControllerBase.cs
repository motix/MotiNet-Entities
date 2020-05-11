using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MotiNet.Entities.Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MotiNet.Entities.Mvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class EntityApiControllerBase<TKey, TEntity, TEntityViewModel, TEntityManager> : ControllerBase
        where TKey : IEquatable<TKey>
        where TEntity : class
        where TEntityViewModel : class
        where TEntityManager : class, IEntityManager<TEntity>
    {
        protected EntityApiControllerBase(TEntityManager entityManager, IMapper mapper)
        {
            EntityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));
            Mapper = mapper;
        }

        protected virtual bool IgnoreDeleteMark => false;

        protected virtual bool IsDeleteMarkEntity => EntityManager is IDeleteMarkEntityManager<TEntity> && !IgnoreDeleteMark;

        protected TEntityManager EntityManager { get; }

        protected IMapper Mapper { get; }

        protected IDeleteMarkEntityManager<TEntity> DeleteMarkEntityManager => EntityManager as IDeleteMarkEntityManager<TEntity>;

        protected virtual Expression<Func<TEntity, object>> EntityIdExpression => null;

        protected virtual Expression<Func<TEntity, bool>> EntityNotDeleteMarkedExpression => throw new NotImplementedException();

        [HttpGet]
        public virtual Task<ActionResult<IEnumerable<TEntityViewModel>>> Get() => GetInternal(x => true);

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TEntityViewModel>> Get(TKey id)
        {
            TEntity model;

            if (EntityIdExpression != null || IsDeleteMarkEntity)
            {
                var spec = new FindSpecification<TEntity>(EntityIdExpression, IsDeleteMarkEntity ? EntityNotDeleteMarkedExpression : null);
                EntitySpecificationAction(spec);
                model = await EntityManager.FindAsync(id, spec);
            }
            else
            {
                model = await FindByIdAsync(id);
            }

            return GetInternal(model);
        }

        [HttpPost]
        public virtual async Task<ActionResult<TEntityViewModel>> Post(TEntityViewModel viewModel)
        {
            var model = Mapper.Map<TEntity>(viewModel);
            ProcessModelForCreate(viewModel, model);
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

            ProcessModelForUpdate(viewModel, model, oldModel);

            var spec = new ModifySpecification<TEntity>();
            EntitySpecificationAction(spec);

            var result = spec.OneToManyRelationships?.Count > 0 || spec.ManyToManyRelationships?.Count > 0 ?
                await EntityManager.UpdateAsync(model, spec) : await EntityManager.UpdateAsync(model);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return await Get(id);
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

        [HttpPost("all")]
        public virtual async Task<ActionResult<UpdateAllViewModel<TKey, TEntityViewModel>>> Post(UpdateAllViewModel<TKey, TEntityViewModel> viewModels)
        {
            foreach (var entry in viewModels.All)
            {
                var oldModel = await FindByIdAsync(entry.Id);

                if (oldModel == null)
                {
                    // Create

                    var result = await Post(entry.ViewModel);
                    switch (result.Result)
                    {
                        case BadRequestObjectResult _:
                            return BadRequest(viewModels);
                        case CreatedAtActionResult _:
                            entry.Result = GenericResult.Success;
                            entry.ViewModel = result.Value;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    // Update

                    var result = await Put(entry.Id, entry.ViewModel);
                    switch (result.Result)
                    {
                        case NotFoundResult _:
                            throw new NotImplementedException();
                        case BadRequestObjectResult actionResult:
                            if (actionResult.Value == null)
                            {
                                return BadRequest();
                            }
                            else
                            {
                                entry.Result = (GenericResult)actionResult.Value;
                                return BadRequest(viewModels);
                            }
                        case null:
                            entry.Result = GenericResult.Success;
                            entry.ViewModel = result.Value;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            if (viewModels.RemoveExtra)
            {
                // Delete

                var ids = viewModels.All.Select(x => x.Id);
                var expression = BuildSearchEntitiesExcludeIdsExpression(ids, viewModels.RemoveExtraParameters);
                var spec = new SearchSpecification<TEntity>(expression);
                var models = await EntityManager.SearchAsync(spec);

                foreach (var model in models)
                {
                    var entry = new UpdateAllEntryViewModel<TKey, TEntityViewModel>
                    {
                        ViewModel = Mapper.Map<TEntityViewModel>(model)
                    };

                    viewModels.All.Add(entry);

                    var result = await Delete(entry.Id);
                    switch (result.Result)
                    {
                        case NotFoundResult _:
                            throw new NotImplementedException();
                        case BadRequestObjectResult actionResult:
                                entry.Result = (GenericResult)actionResult.Value;
                                return BadRequest(viewModels);
                        case null:
                            entry.Result = GenericResult.Success;
                            entry.ViewModel = result.Value;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            return viewModels;
        }

        protected virtual Expression<Func<TEntity, bool>> BuildSearchEntitiesExcludeIdsExpression(IEnumerable<TKey> ids, object parameters)
            => throw new NotImplementedException();

        protected virtual void EntitySpecificationAction(IFindSpecification<TEntity> specification) { }

        protected virtual void EntitySpecificationAction(IModifySpecification<TEntity> specification) { }

        protected virtual void EntitiesSpecificationAction(ISearchSpecification<TEntity> specification) { }

        protected virtual IEnumerable<TEntity> SortEntities(IEnumerable<TEntity> entities) => entities;

        protected virtual void ProcessModelForGet(TEntity model) { }

        protected virtual void ProcessModelsForGet(IEnumerable<TEntity> models) { }

        protected virtual void ProcessViewModelForGet(TEntityViewModel viewModel, TEntity model) { }

        protected virtual void ProcessViewModelsForGet(IEnumerable<TEntityViewModel> viewModels, IEnumerable<TEntity> models) { }

        protected virtual void ProcessModelForCreate(TEntityViewModel viewModel, TEntity model) { }

        protected virtual void ProcessModelForUpdate(TEntityViewModel viewModel, TEntity model, TEntity oldModel) { }

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

        protected virtual async Task<ActionResult<IEnumerable<TEntityViewModel>>> GetInternal(Expression<Func<TEntity, bool>> criteria)
        {
            if (IsDeleteMarkEntity)
            {
                criteria = criteria.And(EntityNotDeleteMarkedExpression);
            }

            var spec = new SearchSpecification<TEntity>(criteria);
            EntitiesSpecificationAction(spec);

            var models = await EntityManager.SearchAsync(spec);

            return GetInternal(models);
        }

        protected virtual ActionResult<TEntityViewModel> GetInternal(TEntity model)
        {
            if (model == null)
            {
                return NotFound();
            }

            ProcessModelForGet(model);
            var viewModel = Mapper.Map<TEntityViewModel>(model);
            ProcessViewModelForGet(viewModel, model);

            return viewModel;
        }

        protected virtual ActionResult<IEnumerable<TEntityViewModel>> GetInternal(IEnumerable<TEntity> models)
        {
            models = SortEntities(models);

            ProcessModelsForGet(models);
            var viewModels = Mapper.Map<List<TEntityViewModel>>(models);
            ProcessViewModelsForGet(viewModels, models);

            return viewModels;
        }
    }
}
