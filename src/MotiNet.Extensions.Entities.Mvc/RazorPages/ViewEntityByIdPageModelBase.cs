using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace MotiNet.Entities.Mvc.RazorPages
{
    public abstract class ViewEntityByIdPageModelBase<TKey, TEntity, TEntityViewModel, TEntityManager> : PageModel
        where TKey : IEquatable<TKey>
        where TEntity : class
        where TEntityViewModel : class
        where TEntityManager : class, IEntityManager<TEntity>
    {
        private readonly IMapper _mapper;

        protected ViewEntityByIdPageModelBase(TEntityManager entityManager, IMapper mapper)
        {
            EntityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected TEntityManager EntityManager { get; }

        public TEntityViewModel Entity { get; set; }

        public async Task<IActionResult> OnGetAsync(TKey id)
        {
            var model = await EntityManager.FindByIdAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            Entity = _mapper.Map<TEntityViewModel>(model);

            return Page();
        }
    }
}
