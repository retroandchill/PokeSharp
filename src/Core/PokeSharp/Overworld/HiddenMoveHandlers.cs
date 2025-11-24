using Injectio.Attributes;
using Microsoft.Extensions.Logging;
using PokeSharp.Core;
using PokeSharp.Core.Strings;
using PokeSharp.PokemonModel;
using PokeSharp.State;
using PokeSharp.UI;
using PokeSharp.UI.Map;

namespace PokeSharp.Overworld;

public interface IHiddenMoveHandler
{
    IEnumerable<Name> MoveIds { get; }
}

public interface ICanUseMoveHiddenMoveHandler : IHiddenMoveHandler
{
    ValueTask<bool> Handle(Name move, Pokemon pokemon, bool showMessage, CancellationToken cancellationToken = default);
}

public interface IConfirmUseMoveHiddenMoveHandler : IHiddenMoveHandler
{
    ValueTask<bool> Handle(Name move, Pokemon pokemon, CancellationToken cancellationToken = default);
}

public interface ISelectOptionBeforeUseHiddenMoveHandler : IHiddenMoveHandler
{
    ValueTask<bool> Handle(
        Name move,
        Pokemon pokemon,
        Func<ValueTask> onCancel,
        CancellationToken cancellationToken = default
    );
}

public interface IUseMoveHiddenMoveHandler : IHiddenMoveHandler
{
    ValueTask<bool> Handle(Name move, Pokemon pokemon, CancellationToken cancellationToken = default);
}

[RegisterSingleton]
[AutoServiceShortcut]
public sealed class HiddenMoveHandlers
{
    private readonly Dictionary<Name, ICanUseMoveHiddenMoveHandler> _canUseMove = new();
    private readonly Dictionary<Name, IConfirmUseMoveHiddenMoveHandler> _confirmUseMove = new();
    private readonly Dictionary<Name, ISelectOptionBeforeUseHiddenMoveHandler> _selectOptionBeforeUse = new();
    private readonly Dictionary<Name, IUseMoveHiddenMoveHandler> _useMove = new();

    public HiddenMoveHandlers(IEnumerable<IHiddenMoveHandler> handlers, ILogger<HiddenMoveHandlers> logger)
    {
        foreach (var handler in handlers)
        {
            switch (handler)
            {
                case ICanUseMoveHiddenMoveHandler canUseMoveHandler:
                    AddHandler(_canUseMove, canUseMoveHandler);
                    break;
                case IConfirmUseMoveHiddenMoveHandler confirmUseMoveHandler:
                    AddHandler(_confirmUseMove, confirmUseMoveHandler);
                    break;
                case ISelectOptionBeforeUseHiddenMoveHandler selectOptionBeforeUseHandler:
                    AddHandler(_selectOptionBeforeUse, selectOptionBeforeUseHandler);
                    break;
                case IUseMoveHiddenMoveHandler useMoveHandler:
                    AddHandler(_useMove, useMoveHandler);
                    break;
                default:
                    logger.LogWarning("Unknown hidden move handler type {Type}", handler.GetType());
                    break;
            }
        }
    }

    private static void AddHandler<T>(Dictionary<Name, T> handlers, T handler)
        where T : IHiddenMoveHandler
    {
        foreach (var id in handler.MoveIds)
        {
            handlers.Add(id, handler);
        }
    }

    public bool HasHandler(Name moveId)
    {
        return _canUseMove.ContainsKey(moveId) || _useMove.ContainsKey(moveId);
    }

    public async ValueTask<bool> CanUseMove(
        Name moveId,
        Pokemon pokemon,
        bool showMessage,
        CancellationToken cancellationToken = default
    )
    {
        return _canUseMove.TryGetValue(moveId, out var handler)
            && await handler.Handle(moveId, pokemon, showMessage, cancellationToken);
    }

    public async ValueTask<bool> ConfirmUseMove(
        Name moveId,
        Pokemon pokemon,
        CancellationToken cancellationToken = default
    )
    {
        if (!_confirmUseMove.TryGetValue(moveId, out var handler))
            return true;

        return await handler.Handle(moveId, pokemon, cancellationToken);
    }

    public async ValueTask<bool> SelectOptionBeforeUse(
        Name moveId,
        Pokemon pokemon,
        Func<ValueTask> onCancel,
        CancellationToken cancellationToken = default
    )
    {
        if (!_selectOptionBeforeUse.TryGetValue(moveId, out var handler))
            return true;

        return await handler.Handle(moveId, pokemon, onCancel, cancellationToken);
    }

    public async ValueTask<bool> UseMove(Name moveId, Pokemon pokemon, CancellationToken cancellationToken = default)
    {
        if (!_useMove.TryGetValue(moveId, out var handler))
            return false;

        return await handler.Handle(moveId, pokemon, cancellationToken);
    }
}

public static class HiddenMoveHandlerExtensions
{
    extension(Pokemon pokemon)
    {
        public ValueTask<bool> CanUseHiddenMove(
            Name moveId,
            bool showMessages = true,
            CancellationToken cancellationToken = default
        )
        {
            return GameGlobal.HiddenMoveHandlers.CanUseMove(moveId, pokemon, showMessages, cancellationToken);
        }

        public ValueTask<bool> ConfirmUseHiddenMove(Name moveId, CancellationToken cancellationToken = default)
        {
            return GameGlobal.HiddenMoveHandlers.ConfirmUseMove(moveId, pokemon, cancellationToken);
        }

        public ValueTask<bool> SelectHiddenMoveOptionBeforeUse(
            Name moveId,
            Func<ValueTask> onCancel,
            CancellationToken cancellationToken = default
        )
        {
            return GameGlobal.HiddenMoveHandlers.SelectOptionBeforeUse(moveId, pokemon, onCancel, cancellationToken);
        }

        public ValueTask<bool> UseHiddenMove(Name moveId, CancellationToken cancellationToken = default)
        {
            return GameGlobal.HiddenMoveHandlers.UseMove(moveId, pokemon, cancellationToken);
        }
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class HiddenMoveFly(IPokemonRegionMapSceneFactory factory, GameTemp gameTemp)
    : ISelectOptionBeforeUseHiddenMoveHandler
{
    private static readonly Name Fly = "FLY";
    public IEnumerable<Name> MoveIds => [Fly];

    async ValueTask<bool> ISelectOptionBeforeUseHiddenMoveHandler.Handle(
        Name move,
        Pokemon pokemon,
        Func<ValueTask> onCancel,
        CancellationToken cancellationToken
    )
    {
        var scene = factory.CreateScene(null, false);
        var screen = new PokemonRegionMapScreen(scene);
        var result = await screen.StartFlyScreen();
        if (result is not null)
        {
            gameTemp.FlyDestination = result;
            return true;
        }

        await onCancel();
        return false;
    }
}
