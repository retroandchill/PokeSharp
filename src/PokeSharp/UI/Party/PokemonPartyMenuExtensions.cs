using PokeSharp.Core;

namespace PokeSharp.UI.Party;

public static class PokemonPartyMenuExtensions
{
    private static CachedService<MenuHandlers<PartyMenuOption, PartyMenuOptionArgs>> _cachedService;

    extension(GameGlobal)
    {
        public static MenuHandlers<PartyMenuOption, PartyMenuOptionArgs> PartyMenuHandlers => _cachedService.Instance;
    }
}
