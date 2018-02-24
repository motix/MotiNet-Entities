using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class TaggedEntityManagerExtensions
    {
        public static ManagerTasks<TEntity> GetManagerTasks<TEntity>()
            where TEntity : class
        {
            return new ManagerTasks<TEntity>()
            {
                EntitySavingAsync = EntitySavingAsync
            };
        }

        private static Task EntitySavingAsync<TEntity>(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
            where TEntity : class
        {
            var taggedManager = (ITaggedEntityManager<TEntity>)manager;

            var tags = taggedManager.TaggedEntityAccessor.GetTags(taskArgs.Entity);
            var normalizedTags = NormalizeEntityTags(taggedManager, tags);
            taggedManager.TaggedEntityAccessor.SetTags(taskArgs.Entity, normalizedTags);

            return Task.FromResult(0);
        }

        private static string NormalizeEntityTags<TEntity>(ITaggedEntityManager<TEntity> manager, string tags)
            where TEntity : class
        {
            return (manager.TagProcessor == null) ? tags : manager.TagProcessor.NormalizeTags(tags);
        }
    }
}
