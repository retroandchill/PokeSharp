using PokeSharp.Core;

namespace PokeSharp.UI.Party;

public class DefaultPartyMenuOptions : IMenuOptionProvider<PartyMenuOption>
{
    public int Priority => 10;

    public IEnumerable<(Name Id, PartyMenuOption Handler)> GetHandlers()
    {
        return [];
    }
}
