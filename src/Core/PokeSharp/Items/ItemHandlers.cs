using Injectio.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokeSharp.BattleSystem;
using PokeSharp.Core;
using PokeSharp.Core.Strings;
using PokeSharp.PokemonModel;
using PokeSharp.Settings;
using PokeSharp.UI;

namespace PokeSharp.Items;

public interface IItemHandler
{
    IEnumerable<Name> ItemIds { get; }
}

public interface IUseTextItemHandler : IItemHandler
{
    Text Handle(Name item);
}

public enum UseFromBagResult
{
    NotUsed,
    Used,
    CloseTheBagToUse,
}

public enum UseInFieldResult : sbyte
{
    ItemEffectNotFound = -1,
    ItemNotUsed = 0,
    ItemUsed = 1,
}

public interface IUseFromBagItemHandler : IItemHandler
{
    ValueTask<UseFromBagResult> Handle(Name item, CancellationToken cancellationToken = default);
}

public interface IConfirmUseInFieldItemHandler : IItemHandler
{
    ValueTask<bool> Handle(Name item, CancellationToken cancellationToken = default);
}

public interface IUseInFieldItemHandler : IItemHandler
{
    ValueTask<bool> Handle(Name item, CancellationToken cancellationToken = default);
}

public interface IUseOnPokemonItemHandler : IItemHandler
{
    ValueTask<bool> Handle(
        Name item,
        int quantity,
        Pokemon pokemon,
        IScreen screen,
        CancellationToken cancellationToken = default
    );
}

public interface IUseOnPokemonMaximumItemHandler : IItemHandler
{
    int Handle(Name item, Pokemon pokemon);
}

public interface ICanUseInBattleItemHandler : IItemHandler
{
    ValueTask<bool> Handle(
        Name item,
        Pokemon pokemon,
        Battler battler,
        BattleMove move,
        bool firstAction,
        Battle battle,
        IScreen screen,
        bool showMessage = true,
        CancellationToken cancellationToken = default
    );
}

public interface IUseInBattleItemHandler : IItemHandler
{
    ValueTask<bool> Handle(Name item, Battler battler, Battle battle, CancellationToken cancellationToken = default);
}

public interface IBattleUseOnPokemonItemHandler : IItemHandler
{
    ValueTask Handle(
        Name item,
        Pokemon pokemon,
        Battler battler,
        IBattleChoices choices,
        IScreen screen,
        CancellationToken cancellationToken = default
    );
}

public interface IBattleUseOnBattlerItemHandler : IItemHandler
{
    ValueTask Handle(Name item, Battler battler, IScreen screen, CancellationToken cancellationToken = default);
}

[RegisterSingleton]
[AutoServiceShortcut]
public sealed class ItemHandlers
{
    private readonly IOptionsMonitor<GameSettings> _gameSettings;
    private readonly Dictionary<Name, IUseTextItemHandler> _useText = new();
    private readonly Dictionary<Name, IUseFromBagItemHandler> _useFromBag = new();
    private readonly Dictionary<Name, IConfirmUseInFieldItemHandler> _confirmUseInField = new();
    private readonly Dictionary<Name, IUseInFieldItemHandler> _useInField = new();
    private readonly Dictionary<Name, IUseOnPokemonItemHandler> _useOnPokemon = new();
    private readonly Dictionary<Name, IUseOnPokemonMaximumItemHandler> _useOnPokemonMaximum = new();
    private readonly Dictionary<Name, ICanUseInBattleItemHandler> _canUseInBattle = new();
    private readonly Dictionary<Name, IUseInBattleItemHandler> _useInBattle = new();
    private readonly Dictionary<Name, IBattleUseOnPokemonItemHandler> _battleUseOnPokemon = new();
    private readonly Dictionary<Name, IBattleUseOnBattlerItemHandler> _battleUseOnBattler = new();

    public ItemHandlers(
        IOptionsMonitor<GameSettings> gameSettings,
        IEnumerable<IItemHandler> handlers,
        ILogger<ItemHandlers> logger
    )
    {
        _gameSettings = gameSettings;
        foreach (var handler in handlers)
        {
            switch (handler)
            {
                case IUseTextItemHandler useTextHandler:
                    AddHandler(_useText, useTextHandler);
                    break;
                case IUseFromBagItemHandler bagHandler:
                    AddHandler(_useFromBag, bagHandler);
                    break;
                case IConfirmUseInFieldItemHandler confirmUseInFieldHandler:
                    AddHandler(_confirmUseInField, confirmUseInFieldHandler);
                    break;
                case IUseInFieldItemHandler useInFieldHandler:
                    AddHandler(_useInField, useInFieldHandler);
                    break;
                case IUseOnPokemonItemHandler useOnPokemonHandler:
                    AddHandler(_useOnPokemon, useOnPokemonHandler);
                    break;
                case IUseOnPokemonMaximumItemHandler useOnPokemonMaximumHandler:
                    AddHandler(_useOnPokemonMaximum, useOnPokemonMaximumHandler);
                    break;
                case ICanUseInBattleItemHandler canUseInBattleItemHandler:
                    AddHandler(_canUseInBattle, canUseInBattleItemHandler);
                    break;
                case IUseInBattleItemHandler useInBattleItemHandler:
                    AddHandler(_useInBattle, useInBattleItemHandler);
                    break;
                case IBattleUseOnPokemonItemHandler battleUseOnPokemonItemHandler:
                    AddHandler(_battleUseOnPokemon, battleUseOnPokemonItemHandler);
                    break;
                case IBattleUseOnBattlerItemHandler battleUseOnBattlerItemHandler:
                    AddHandler(_battleUseOnBattler, battleUseOnBattlerItemHandler);
                    break;
                default:
                    logger.LogUnknownItemHandlerType(handler.GetType());
                    break;
            }
        }
    }

    private static void AddHandler<T>(Dictionary<Name, T> handlers, T handler)
        where T : IItemHandler
    {
        foreach (var id in handler.ItemIds)
        {
            handlers.Add(id, handler);
        }
    }

    public bool HasUseText(Name item) => _useText.ContainsKey(item);

    public bool HasOutHandler(Name item) =>
        _useFromBag.ContainsKey(item) || _useInField.ContainsKey(item) || _useOnPokemon.ContainsKey(item);

    public bool HasUseInFieldHandler(Name item) => _useInField.ContainsKey(item);

    public bool HasUseOnPokemon(Name item) => _useOnPokemon.ContainsKey(item);

    public bool HasUseOnPokemonMaximum(Name item) => _useOnPokemonMaximum.ContainsKey(item);

    public bool HasUseInBattle(Name item) => _useInBattle.ContainsKey(item);

    public bool HasBattleUseOnBattler(Name item) => _battleUseOnBattler.ContainsKey(item);

    public bool HasBattleUseOnPokemon(Name item) => _battleUseOnPokemon.ContainsKey(item);

    public Text GetUseText(Name item) => _useText[item].Handle(item);

    public bool TryGetUseText(Name item, out Text text)
    {
        if (_useText.TryGetValue(item, out var handler))
        {
            text = handler.Handle(item);
            return true;
        }

        text = Text.None;
        return false;
    }

    public async ValueTask<UseFromBagResult> UseFromBag(Name item, CancellationToken cancellationToken = default)
    {
        if (_useFromBag.TryGetValue(item, out var handler))
        {
            return await handler.Handle(item, cancellationToken);
        }

        if (_useInField.TryGetValue(item, out var fieldHandler))
        {
            return await fieldHandler.Handle(item, cancellationToken)
                ? UseFromBagResult.Used
                : UseFromBagResult.NotUsed;
        }

        return UseFromBagResult.NotUsed;
    }

    public async ValueTask<bool> ConfirmUseInField(Name item, CancellationToken cancellationToken = default)
    {
        if (!_confirmUseInField.TryGetValue(item, out var handler))
            return true;

        return await handler.Handle(item, cancellationToken);
    }

    public async ValueTask<UseInFieldResult> UseInField(Name item, CancellationToken cancellationToken = default)
    {
        if (!_useInField.TryGetValue(item, out var handler))
            return UseInFieldResult.ItemEffectNotFound;

        return await handler.Handle(item, cancellationToken) ? UseInFieldResult.ItemUsed : UseInFieldResult.ItemNotUsed;
    }

    public async ValueTask<bool> UseOnPokemon(
        Name item,
        int quantity,
        Pokemon pokemon,
        IScreen screen,
        CancellationToken cancellationToken = default
    )
    {
        if (!_useOnPokemon.TryGetValue(item, out var handler))
            return false;

        return await handler.Handle(item, quantity, pokemon, screen, cancellationToken);
    }

    public int GetUseOnPokemonMaximum(Name item, Pokemon pokemon)
    {
        if (
            !_useOnPokemonMaximum.TryGetValue(item, out var handler)
            || !_gameSettings.CurrentValue.UseMultipleStatItemsAtOnce
        )
        {
            return 1;
        }

        return handler.Handle(item, pokemon);
    }

    public async ValueTask<bool> CanUseInBattle(
        Name item,
        Pokemon pokemon,
        Battler battler,
        BattleMove move,
        bool firstAction,
        Battle battle,
        IScreen screen,
        bool showMessage = true,
        CancellationToken cancellationToken = default
    )
    {
        if (!_canUseInBattle.TryGetValue(item, out var handler))
            return false;

        return await handler.Handle(
            item,
            pokemon,
            battler,
            move,
            firstAction,
            battle,
            screen,
            showMessage,
            cancellationToken
        );
    }

    public async ValueTask UseInBattle(
        Name item,
        Battler battler,
        Battle battle,
        CancellationToken cancellationToken = default
    )
    {
        if (!_useInBattle.TryGetValue(item, out var handler))
            return;

        await handler.Handle(item, battler, battle, cancellationToken);
    }

    public async ValueTask BattleUseOnBattler(
        Name item,
        Battler battler,
        IScreen screen,
        CancellationToken cancellationToken = default
    )
    {
        if (!_battleUseOnBattler.TryGetValue(item, out var handler))
            return;

        await handler.Handle(item, battler, screen, cancellationToken);
    }

    public async ValueTask BattleUseOnPokemon(
        Name item,
        Pokemon pokemon,
        Battler battler,
        IBattleChoices choices,
        IScreen screen,
        CancellationToken cancellationToken = default
    )
    {
        if (!_battleUseOnPokemon.TryGetValue(item, out var handler))
            return;

        await handler.Handle(item, pokemon, battler, choices, screen, cancellationToken);
    }
}

internal static partial class ItemHandlersLogs
{
    [LoggerMessage(LogLevel.Warning, "Unknown item handler type: {Type}")]
    public static partial void LogUnknownItemHandlerType(this ILogger logger, Type type);
}
