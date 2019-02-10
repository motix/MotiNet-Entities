using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class TaggedEntityManagerExtensions<TEntity>
        where TEntity : class
    {
        public static ManagerTasks<TEntity> GetManagerTasks()
        {
            return new ManagerTasks<TEntity>()
            {
                EntitySavingAsync = EntitySavingAsync
            };
        }

        private static Task EntitySavingAsync(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
        {
            var taggedManager = (ITaggedEntityManager<TEntity>)manager;

            var tags = taggedManager.TaggedEntityAccessor.GetTags(taskArgs.Entity);
            var normalizedTags = NormalizeEntityTags(taggedManager, tags);
            taggedManager.TaggedEntityAccessor.SetTags(taskArgs.Entity, normalizedTags);

            return Task.FromResult(0);
        }

        private static string NormalizeEntityTags(ITaggedEntityManager<TEntity> manager, string tags)
        {
            return (manager.TagProcessor == null) ? tags : manager.TagProcessor.NormalizeTags(tags);
        }
    }
}
