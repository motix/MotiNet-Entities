namespace MotiNet.Entities
{
    public static class TaggedEntityManagerExtensions
    {
        public static ManagerEventHandlers<TEntity> GetManagerEventHandlers<TEntity>()
            where TEntity : class
        {
            return new ManagerEventHandlers<TEntity>()
            {
                EntityPreparingForSaving = PrepareEntityForSaving
            };
        }

        private static void PrepareEntityForSaving<TEntity>(object sender, ManagerEventArgs<TEntity> e)
            where TEntity : class
        {
            var manager = (ITaggedEntityManager<TEntity>)sender;

            var tags = manager.TaggedEntityAccessor.GetTags(e.Entity);
            var normalizedTags = NormalizeEntityTags(manager, tags);
            manager.TaggedEntityAccessor.SetTags(e.Entity, normalizedTags);
        }

        private static string NormalizeEntityTags<TEntity>(ITaggedEntityManager<TEntity> manager, string tags)
            where TEntity : class
        {
            return (manager.TagProcessor == null) ? tags : manager.TagProcessor.NormalizeTags(tags);
        }
    }
}
