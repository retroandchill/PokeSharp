using PokeSharp.Core;

namespace PokeSharp.UI.Pause;

public static class PokemonPauseMenuExtensions
{
    private static CachedService<MenuHandlers<PauseMenuOption, NullContext>> _cachedService;

    extension(GameGlobal)
    {
        public static MenuHandlers<PauseMenuOption, NullContext> PauseMenuHandlers => _cachedService.Instance;
    }
}
