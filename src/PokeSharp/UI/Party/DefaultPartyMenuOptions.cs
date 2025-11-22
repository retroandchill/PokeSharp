using Injectio.Attributes;

namespace PokeSharp.UI.Party;

[RegisterSingleton<IMenuOptionProvider<PartyMenuOption>>]
[MenuOptionRegistration<PartyMenuOption>]
public sealed partial class DefaultPartyMenuOptions;
