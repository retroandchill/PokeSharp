using System.Collections.Immutable;
using Injectio.Attributes;
using PokeSharp.Audio;
using PokeSharp.Core;
using PokeSharp.Core.Engine;
using PokeSharp.Core.State;
using PokeSharp.Items;
using PokeSharp.Overworld;
using PokeSharp.Services;
using PokeSharp.State;
using PokeSharp.Trainers;
using Retro.ReadOnlyParams.Annotations;

namespace PokeSharp.UI;

public sealed record PauseMenuOption : IMenuOption<NullContext>
{
    public required HandlerName Name { get; init; }

    public required int? Order { get; init; }

    public Func<NullContext, bool>? Condition { get; init; }

    public required Func<IPokemonPauseMenuScene, CancellationToken, ValueTask<bool>> Effect { get; init; }
}

public interface IPokemonPauseMenuScene
{
    void StartScene();

    void ShowInfo(Text text);

    void ShowMenu();

    void HideMenu();

    ValueTask<int?> ShowCommands(IReadOnlyList<Text> handlers, CancellationToken cancellationToken = default);

    void EndScene();

    void Refresh();
}

public class PokemonPauseScreen([ReadOnly] IPokemonPauseMenuScene scene) : IScreen
{
    public void ShowMenu()
    {
        scene.Refresh();
        scene.ShowMenu();
    }

    private void ShowInfo()
    {
        GameGlobal.PauseMenuService.ShowInfo(this);
    }

    public async ValueTask StartPokemonMenu(CancellationToken cancellationToken = default)
    {
        scene.StartScene();
        ShowInfo();
        var commandList = new List<Text>();
        var commands = new List<PauseMenuOption>();
        foreach (var (_, handler, name) in GameGlobal.PauseMenuHandlers.GetAllAvailable())
        {
            commandList.Add(name);
            commands.Add(handler);
        }

        var endScene = false;
        while (true)
        {
            var choice = await scene.ShowCommands(commandList, cancellationToken);
            if (!choice.HasValue)
            {
                GameGlobal.AudioPlayService.PlayCloseMenuSE();
                endScene = true;
                break;
            }

            if (await commands[choice.Value].Effect(scene, cancellationToken))
            {
                break;
            }
        }

        if (endScene)
        {
            scene.EndScene();
        }
    }
}

public sealed class DefaultPauseMenuCommands(
    IEngineInteropService engineInteropService,
    IAudioPlayService audioPlayService,
    IGameStateAccessor<PlayerTrainer> playerTrainer,
    IPokemonPartySceneFactory partySceneFactory,
    GameTemp gameTemp,
    HiddenMoveHandlers hiddenMoveHandlers,
    IGameStateAccessor<PokemonGlobal> pokemonGlobal,
    IGameStateAccessor<PokemonBag> bag,
    IGameStateAccessor<GameSystem> gameSystem
) : IRegistrationProvider<PauseMenuOption>
{
    private static readonly Name TownMap = "TOWNMAP";

    public int Priority => 0;

    public IEnumerable<(Name Id, PauseMenuOption Handler)> GetHandlers()
    {
        yield return (
            "Pokedex",
            new PauseMenuOption
            {
                Name = Text.Localized("PauseMenu.Pokedex", "Pokedex", "Pokédex"),
                Order = 10,
                Condition = _ => playerTrainer.Current is { HasPokedex: true, Pokedex.AccessibleDexes.Count: > 1 },
                Effect = (menu, ct) => throw new NotImplementedException(),
            }
        );

        yield return (
            "Party",
            new PauseMenuOption
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
            }
        );

        yield return (
            "Bag",
            new PauseMenuOption
            {
                Name = Text.Localized("PauseMenu.Pokedex", "Bag", "Bag"),
                Order = 30,
                Condition = _ => !pokemonGlobal.Current.BugContestState.InProgress,
                Effect = (menu, ct) => throw new NotImplementedException(),
            }
        );

        yield return (
            "Pokegear",
            new PauseMenuOption
            {
                Name = Text.Localized("PauseMenu.Pokegear", "Pokegear", "Pokégear"),
                Order = 40,
                Condition = _ => playerTrainer.Current.HasPokegear,
                Effect = (menu, ct) => throw new NotImplementedException(),
            }
        );

        yield return (
            "TownMap",
            new PauseMenuOption
            {
                Name = Text.Localized("PauseMenu.TownMap", "TownMap", "Town Map"),
                Order = 40,
                Condition = _ => !playerTrainer.Current.HasPokegear && bag.Current.Has(TownMap),
                Effect = (menu, ct) => throw new NotImplementedException(),
            }
        );

        yield return (
            "TrainerCard",
            new PauseMenuOption
            {
                Name = HandlerName.FromDelegate(() => playerTrainer.Current.Name),
                Order = 50,
                Condition = _ => GameGlobal.PlayerTrainer is { HasPokedex: true, Pokedex.AccessibleDexes.Count: > 1 },
                Effect = (menu, ct) => throw new NotImplementedException(),
            }
        );

        yield return (
            "Save",
            new PauseMenuOption
            {
                Name = Text.Localized("PauseMenu.Save", "Save", "Save"),
                Order = 60,
                Condition = _ =>
                    !gameSystem.Current.SaveDisabled
                    && pokemonGlobal.Current is { BugContestState.InProgress: false, SafariState.InProgress: false },
                Effect = (menu, ct) => throw new NotImplementedException(),
            }
        );

        yield return (
            "Options",
            new PauseMenuOption
            {
                Name = Text.Localized("PauseMenu.Options", "Options", "Options"),
                Order = 70,
                Effect = (menu, ct) => throw new NotImplementedException(),
            }
        );

        yield return (
            "QuitGame",
            new PauseMenuOption
            {
                Name = Text.Localized("PauseMenu.QuitGame", "QuitGame", "Quit Game"),
                Order = 90,
                Condition = _ => GameGlobal.PlayerTrainer is { HasPokedex: true, Pokedex.AccessibleDexes.Count: > 1 },
                Effect = (menu, ct) => throw new NotImplementedException(),
            }
        );
    }
}

public static class PokemonPauseMenuExtensions
{
    private static CachedService<MenuHandlers<PauseMenuOption, NullContext>> _cachedService;

    extension(GameGlobal)
    {
        public static MenuHandlers<PauseMenuOption, NullContext> PauseMenuHandlers => _cachedService.Instance;
    }
}

public interface IPauseSceneInfoProvider
{
    int Order { get; }

    bool ShowInfo(PokemonPauseScreen pauseScreen);
}

[RegisterSingleton]
[AutoServiceShortcut]
public sealed class PauseMenuService(IEnumerable<IPauseSceneInfoProvider> providers)
{
    private readonly ImmutableArray<IPauseSceneInfoProvider> _providers = [.. providers.OrderBy(x => x.Order)];

    public void ShowInfo(PokemonPauseScreen pauseScreen)
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
