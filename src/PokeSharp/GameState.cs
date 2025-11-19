using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using PokeSharp.Core.Saving;
using Retro.ReadOnlyParams.Annotations;
using Zomp.SyncMethodGenerator;

namespace PokeSharp;

[RegisterSingleton]
[AutoServiceShortcut]
public partial class GameState([ReadOnly] DataService dataService, [ReadOnly] SaveDataService saveDataService)
{
    [CreateSyncVersion]
    public async ValueTask InitializeAsync(CancellationToken cancellationToken = default)
    {
        await dataService.LoadPreliminaryDataAsync(cancellationToken);
        await dataService.LoadGameDataAsync(cancellationToken);
    }

    [CreateSyncVersion]
    public async ValueTask SetUpSystemAsync(CancellationToken cancellationToken = default)
    {
        var saveData = saveDataService.Exists
            ? await saveDataService.ReadDataFromFileAsync(saveDataService.FilePath, cancellationToken)
            : new Dictionary<Name, object>();
        if (saveData.Count == 0)
        {
            saveDataService.InitializeBootupValues();
        }
        else
        {
            saveDataService.LoadBootupValues(saveData);
        }

        // TODO: We need to prompt for the language choice here
    }
}
