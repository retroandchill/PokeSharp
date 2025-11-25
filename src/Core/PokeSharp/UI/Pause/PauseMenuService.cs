using System.Collections.Immutable;
using Injectio.Attributes;
using PokeSharp.Core;

namespace PokeSharp.UI.Pause;

[RegisterSingleton]
[AutoServiceShortcut]
public sealed class PauseMenuService(IEnumerable<IPauseSceneInfoProvider> providers)
{
    private readonly ImmutableArray<IPauseSceneInfoProvider> _providers = [.. providers.OrderBy(x => x.Order)];

    public void ShowInfo(IPokemonPauseMenuScene pauseScreen)
    {
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var provider in _providers)
        {
            if (provider.ShowInfo(pauseScreen))
            {
                return;
            }
        }
    }
}
