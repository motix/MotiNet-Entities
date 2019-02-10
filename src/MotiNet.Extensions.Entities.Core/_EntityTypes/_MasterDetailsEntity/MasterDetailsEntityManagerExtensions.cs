using System.Collections.Generic;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class MasterDetailsEntityManagerExtensions<TEntity, TEntityDetail>
        where TEntity : class
        where TEntityDetail : class
    {
        public static ManagerTasks<TEntity, TEntityDetail> GetManagerTasks()
        {
            return new ManagerTasks<TEntity, TEntityDetail>()
            {
                EntityWithSubEntityValidateAsync = EntityValidateAsync
            };
        }

        private static async Task EntityValidateAsync(IManager<TEntity, TEntityDetail> manager, ValidateEntityTaskArgs<TEntity, TEntityDetail> taskArgs)
        {
            var masterDetailsManager = (IMasterDetailsEntityManager<TEntity, TEntityDetail>)manager;

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
