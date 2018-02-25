using System.Collections.Generic;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class MasterDetailsEntityManagerExtensions
    {
        public static ManagerTasks<TEntity, TSubEntity> GetManagerTasks<TEntity, TSubEntity>()
            where TEntity : class
            where TSubEntity : class
        {
            return new ManagerTasks<TEntity, TSubEntity>()
            {
                EntityWithSubEntitiesValidateAsync = EntityWithSubEntitiesValidateAsync
            };
        }

        private static async Task EntityWithSubEntitiesValidateAsync<TEntity, TSubEntity>(IManager<TEntity, TSubEntity> manager, ValidateEntityTaskArgs<TEntity, TSubEntity> taskArgs)
            where TEntity : class
            where TSubEntity : class
        {
            var masterDetailsManager = (IMasterDetailsEntityManager<TEntity, TSubEntity>)manager;

            var details = masterDetailsManager.MasterDetailsEntityAccessor.GetDetails(taskArgs.Entity);
            var tasks = new List<Task<GenericResult>>(details.Count);
            foreach (var detail in details)
            {
                tasks.Add(masterDetailsManager.ValidateSubEntityAsync(detail));
            }
            await Task.WhenAll(tasks);

            foreach (var task in tasks)
            {
                taskArgs.Errors.AddRange(task.Result.Errors);
            }
        }
    }
}
