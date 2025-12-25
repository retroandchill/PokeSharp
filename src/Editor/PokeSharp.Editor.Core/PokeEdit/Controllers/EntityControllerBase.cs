using PokeSharp.Core.Data;
using PokeSharp.Editor.Core.PokeEdit.Editors;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit.Controllers;

public abstract class EntityControllerBase<TKey, TEntity>(IEntityRepository<TKey, TEntity> repository)
    where TKey : notnull
    where TEntity : ILoadedGameDataEntity<TKey, TEntity>
{
    protected IEntityRepository<TKey, TEntity> Repository { get; } = repository;

    [PokeEditRequest]
    public TEntity GetEntry(TKey key)
    {
        return Repository.GetEntry(key);
    }

    [PokeEditRequest]
    public TEntity GetEntryAt(int index)
    {
        return Repository.GetEntryAt(index);
    }

    [PokeEditRequest]
    public EntityUpdateResponse? ApplyEdit(TKey key, ObjectDiffNode diff)
    {
        return new EntityUpdateResponse(Repository.ApplyEdit(key, diff));
    }

    [PokeEditRequest]
    public EntityUpdateResponse ApplyEditAt(int index, ObjectDiffNode diff)
    {
        return new EntityUpdateResponse(Repository.ApplyEditAt(index, diff));
    }

    [PokeEditRequest]
    public void Swap(int index1, int index2)
    {
        Repository.Swap(index1, index2);
    }

    [PokeEditRequest]
    public void Remove(TKey key)
    {
        Repository.Remove(key);
    }

    [PokeEditRequest]
    public void RemoveAt(int index)
    {
        Repository.RemoveAt(index);
    }

    public void SaveChanges()
    {
        Repository.SaveChanges();
    }

    [PokeEditRequest]
    public async ValueTask SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await Repository.SaveChangesAsync(cancellationToken);
    }
}
