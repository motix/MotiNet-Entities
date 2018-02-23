namespace MotiNet.Entities
{
    public interface ICodeBasedEntityAccessor<TEntity>
        where TEntity : class
    {
        string GetCode(TEntity entity);

        void SetCode(TEntity entity, string code);
    }
}
