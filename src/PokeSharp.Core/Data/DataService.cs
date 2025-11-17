using System.Collections.Immutable;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Core.Data;

[RegisterSingleton]
public partial class DataService(IEnumerable<IDataRepository> repositories)
{
    private readonly ImmutableArray<IDataRepository> _dataRepositories = [.. repositories];

    [CreateSyncVersion]
    public async ValueTask LoadAllAsync(CancellationToken cancellationToken = default)
    {
        foreach (var repository in _dataRepositories)
        {
            await repository.LoadAsync(cancellationToken);
        }
    }
}
