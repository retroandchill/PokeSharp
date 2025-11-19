using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using Retro.ReadOnlyParams.Annotations;
using Zomp.SyncMethodGenerator;

namespace PokeSharp;

[RegisterSingleton]
[AutoServiceShortcut]
public partial class GameState([ReadOnly] DataService dataService)
{
    [CreateSyncVersion]
    public async ValueTask InitializeAsync(CancellationToken cancellationToken = default)
    {
        await dataService.LoadPreliminaryDataAsync(cancellationToken);
        await dataService.LoadGameDataAsync(cancellationToken);
    }
}
