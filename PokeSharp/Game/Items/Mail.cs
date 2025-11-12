using PokeSharp.Abstractions;
using PokeSharp.Data.Core;

namespace PokeSharp.Game.Items;

public record MailPokemonInfo(Name Species, PokemonGender Gender, bool Shiny, int Form, bool Shadow, bool IsEgg);

public record Mail(
    Name Item,
    Text Message,
    Text Sender,
    MailPokemonInfo? Pokemon1,
    MailPokemonInfo? Pokemon2,
    MailPokemonInfo? Pokemon3
);
