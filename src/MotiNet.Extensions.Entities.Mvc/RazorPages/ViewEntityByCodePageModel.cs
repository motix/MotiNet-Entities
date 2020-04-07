using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace MotiNet.Entities.Mvc.RazorPages
{
    public class ViewEntityByCodePageModel<TEntity, TEntityViewModel, TEntityManager> : PageModel
        where TEntity : class
        where TEntityViewModel : class
        where TEntityManager : class, ICodeBasedEntityManager<TEntity>
    {
        private readonly IMapper _mapper;

        public ViewEntityByCodePageModel(TEntityManager entityManager, IMapper mapper)
        {
            EntityManager = entityManager ?? throw new ArgumentNullException(nameof(entityManager));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected TEntityManager EntityManager { get; }

        public TEntityViewModel Entity { get; set; }

        public async Task<IActionResult> OnGetAsync(string code)
        {
            var model = await EntityManager.FindByCodeAsync(code);

            if (model == null)
            {
                return NotFound();
            }

            Entity = _mapper.Map<TEntityViewModel>(model);

            return Page();
        }
    }
}
