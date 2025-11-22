using PokeSharp.Core;

namespace PokeSharp.UI.Party;

[AutoServiceShortcut]
public interface IPokemonPartySceneFactory
{
    IPokemonPartyScene Create();
}
