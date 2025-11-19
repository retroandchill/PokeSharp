using System.Collections.Immutable;
using Microsoft.Extensions.Options;
using Retro.ReadOnlyParams.Annotations;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Core.Data;

[RegisterSingleton]
public partial class DataService([ReadOnly] IOptionsMonitor<DataSettings> dataSettings, IEnumerable<ILoadedGameDataSet> gameData, IEnumerable<IDataRepository> additionalData)
{
    private readonly ImmutableArray<ILoadedGameDataSet> _gameData = [.. gameData];
    private readonly ImmutableArray<IDataRepository> _additionalData = [..additionalData];

    public IEnumerable<(string FileName, bool IsMandatory)> GetAllDataFilenames()
    {
        var basePath = dataSettings.CurrentValue.DataFileBasePath;
        return _gameData.Select(r => (Path.Combine(basePath, $"{r.DataPath}.pkdata"), !r.IsOptional));
    }
    
    [CreateSyncVersion]
    public async ValueTask LoadGameDataAsync(CancellationToken cancellationToken = default)
    {
        foreach (var repository in _gameData)
        {
            await repository.LoadAsync(cancellationToken);
        }
    }
    
    [CreateSyncVersion]
    public async ValueTask SaveGameDataAsync(CancellationToken cancellationToken = default)
    {
        foreach (var repository in _gameData)
        {
            await repository.SaveAsync(cancellationToken);
        }
    }

    [CreateSyncVersion]
    public async ValueTask LoadPreliminaryDataAsync(CancellationToken cancellationToken = default)
    {
        foreach (var repository in _additionalData)
        {
            await repository.LoadAsync(cancellationToken);
        }
    }
}
