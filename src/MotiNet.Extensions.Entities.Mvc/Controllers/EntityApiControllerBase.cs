using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MotiNet.Entities;
using System;
using System.Collections.Generic;
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

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TEntityViewModel>>> Get()
        {
            var models = await EntityManager.AllAsync();
            return Mapper.Map<List<TEntityViewModel>>(models);
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TEntityViewModel>> Get(TKey id)
        {
            var model = await EntityManager.FindByIdAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            return Mapper.Map<TEntityViewModel>(model);
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

        protected virtual async Task<bool> EntityExists(TKey id)
        {
            var model = await EntityManager.FindByIdAsync(id);
            return model != null;
        }
    }
}
