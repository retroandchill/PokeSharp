using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using PokeSharp.Core.Saving;
using PokeSharp.Core.State;
using PokeSharp.Core.Strings;
using PokeSharp.State;
using Retro.ReadOnlyParams.Annotations;
using Zomp.SyncMethodGenerator;

namespace PokeSharp;

[RegisterSingleton]
[AutoServiceShortcut]
public partial class GameState(
    [ReadOnly] DataService dataService,
    [ReadOnly] SaveDataService saveDataService,
    [ReadOnly] GameTemp gameTemp,
    [ReadOnly] IGameStateAccessor<GameStats> gameStats
)
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

    [CreateSyncVersion]
    public async ValueTask StartNewAsync(CancellationToken cancellationToken = default)
    {
        gameTemp.CommonEventId = 0;
        gameTemp.BegunNewGame = true;
        saveDataService.LoadNewGameValues();
        gameStats.Current.PlaySessions++;

        // TODO: Basic map setup
    }
}
