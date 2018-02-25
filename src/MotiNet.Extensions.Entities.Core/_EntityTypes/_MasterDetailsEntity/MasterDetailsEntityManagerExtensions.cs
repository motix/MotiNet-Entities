using System.Collections.Generic;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class MasterDetailsEntityManagerExtensions
    {
        public static ManagerTasks<TEntity, TEntityDetail> GetManagerTasks<TEntity, TEntityDetail>()
            where TEntity : class
            where TEntityDetail : class
        {
            return new ManagerTasks<TEntity, TEntityDetail>()
            {
                EntityWithSubEntityValidateAsync = EntityValidateAsync
            };
        }

        private static async Task EntityValidateAsync<TEntity, TEntityDetail>(IManager<TEntity, TEntityDetail> manager, ValidateEntityTaskArgs<TEntity, TEntityDetail> taskArgs)
            where TEntity : class
            where TEntityDetail : class
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
