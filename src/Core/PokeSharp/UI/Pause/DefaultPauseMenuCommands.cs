using Injectio.Attributes;
using PokeSharp.Audio;
using PokeSharp.Core;
using PokeSharp.Core.State;
using PokeSharp.Items;
using PokeSharp.Overworld;
using PokeSharp.Services;
using PokeSharp.State;
using PokeSharp.Trainers;
using PokeSharp.UI.Party;

namespace PokeSharp.UI.Pause;

[RegisterSingleton<IMenuOptionProvider<PauseMenuOption>>]
[MenuOptionRegistration<PauseMenuOption>]
public sealed partial class DefaultPauseMenuCommands(
    IAudioPlayService audioPlayService,
    IGameStateAccessor<PlayerTrainer> playerTrainer,
    IPokemonPartySceneFactory partySceneFactory,
    GameTemp gameTemp,
    HiddenMoveHandlers hiddenMoveHandlers,
    IGameStateAccessor<PokemonGlobal> pokemonGlobal,
    IGameStateAccessor<PokemonBag> bag,
    IGameStateAccessor<GameSystem> gameSystem
)
{
    [MenuOption("Pokedex")]
    private readonly PauseMenuOption _pokedex = new()
    {
        Name = Text.Localized("PauseMenu.Pokedex", "Pokedex", "Pokédex"),
        Order = 10,
        Condition = _ => playerTrainer.Current is { HasPokedex: true, Pokedex.AccessibleDexes.Count: > 1 },
        Effect = (menu, ct) => throw new NotImplementedException(),
    };

    [MenuOption("Party")]
    private readonly PauseMenuOption _party = new()
    {
        Name = Text.Localized("PauseMenu.Party", "Party", "Pokémon"),
        Order = 20,
        Condition = _ => playerTrainer.Current.PartyCount > 1,
        Effect = async (menu, ct) =>
        {
            audioPlayService.PlayDecisionSE();
            var scene = partySceneFactory.Create();
            var screen = new PokemonPartyScreen(scene, playerTrainer.Current.Party);
            var hiddenMove = await screen.PokemonScreen(ct);
            if (hiddenMove is not null)
            {
                menu.EndScene();
            }
            else
            {
                menu.Refresh();
                return false;
            }

            gameTemp.InMenu = false;
            var (pokemon, move) = hiddenMove.Value;
            await hiddenMoveHandlers.UseMove(move, pokemon, ct);
            return true;
        },
    };

    [MenuOption("Party")]
    private readonly PauseMenuOption _bag = new()
    {
        Name = Text.Localized("PauseMenu.Pokedex", "Bag", "Bag"),
        Order = 30,
        Condition = _ => !pokemonGlobal.Current.BugContestState.InProgress,
        Effect = (menu, ct) => throw new NotImplementedException(),
    };

    [MenuOption("Pokegear")]
    private readonly PauseMenuOption _pokegear = new()
    {
        Name = Text.Localized("PauseMenu.Pokegear", "Pokegear", "Pokégear"),
        Order = 40,
        Condition = _ => playerTrainer.Current.HasPokegear,
        Effect = (menu, ct) => throw new NotImplementedException(),
    };

    private static readonly Name TownMap = "TOWNMAP";

    [MenuOption("TownMap")]
    private readonly PauseMenuOption _townMap = new()
    {
        Name = Text.Localized("PauseMenu.TownMap", "TownMap", "Town Map"),
        Order = 40,
        Condition = _ => !playerTrainer.Current.HasPokegear && bag.Current.Has(TownMap),
        Effect = (menu, ct) => throw new NotImplementedException(),
    };

    [MenuOption("TrainerCard")]
    private readonly PauseMenuOption _trainerCard = new()
    {
        Name = HandlerName.FromDelegate(() => playerTrainer.Current.Name),
        Order = 50,
        Condition = _ => GameGlobal.PlayerTrainer is { HasPokedex: true, Pokedex.AccessibleDexes.Count: > 1 },
        Effect = (menu, ct) => throw new NotImplementedException(),
    };

    [MenuOption("Save")]
    private readonly PauseMenuOption _save = new()
    {
        Name = Text.Localized("PauseMenu.Save", "Save", "Save"),
        Order = 60,
        Condition = _ =>
            !gameSystem.Current.SaveDisabled
            && pokemonGlobal.Current is { BugContestState.InProgress: false, SafariState.InProgress: false },
        Effect = (menu, ct) => throw new NotImplementedException(),
    };

    [MenuOption("Options")]
    private readonly PauseMenuOption _options = new()
    {
        Name = Text.Localized("PauseMenu.Options", "Options", "Options"),
        Order = 70,
        Effect = (menu, ct) => throw new NotImplementedException(),
    };

    [MenuOption("QuitGame")]
    private readonly PauseMenuOption _quitGame = new()
    {
        Name = Text.Localized("PauseMenu.QuitGame", "QuitGame", "Quit Game"),
        Order = 90,
        Condition = _ => GameGlobal.PlayerTrainer is { HasPokedex: true, Pokedex.AccessibleDexes.Count: > 1 },
        Effect = (menu, ct) => throw new NotImplementedException(),
    };
}
