using MessagePack;
using PokeSharp.Core;
using PokeSharp.Core.Strings;
using PokeSharp.Data.Core;

namespace PokeSharp.Items;

[MessagePackObject(true)]
public record MailPokemonInfo(Name Species, PokemonGender Gender, bool Shiny, int Form, bool Shadow, bool IsEgg);

[MessagePackObject(true)]
public record Mail(
    Name Item,
    Text Message,
    Text Sender,
    MailPokemonInfo? Pokemon1 = null,
    MailPokemonInfo? Pokemon2 = null,
    MailPokemonInfo? Pokemon3 = null
);

[AutoServiceShortcut]
public interface IMailService
{
    ValueTask DisplayMail(Mail mail, CancellationToken cancellationToken = default);
}
