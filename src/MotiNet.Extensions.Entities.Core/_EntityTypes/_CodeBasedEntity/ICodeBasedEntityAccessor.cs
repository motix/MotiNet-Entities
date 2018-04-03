namespace MotiNet.Entities
{
    public interface ICodeBasedEntityAccessor<TEntity>
        where TEntity : class
    {
        object GetId(TEntity entity);

        string GetCode(TEntity entity);

        void SetCode(TEntity entity, string code);
    }
}
