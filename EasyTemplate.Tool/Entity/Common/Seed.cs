namespace EasyTemplate.Tool.Entity.Common;

public interface ISeedData<TEntity> where TEntity : class, new()
{
    IEnumerable<TEntity> Generate();
}
