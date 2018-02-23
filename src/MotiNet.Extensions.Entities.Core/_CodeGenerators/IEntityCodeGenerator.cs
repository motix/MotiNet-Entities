namespace MotiNet.Entities
{
    public interface IEntityCodeGenerator<in TEntity>
        where TEntity : class
    {
        string GenerateCode(object manager, TEntity entity);
    }
}
