using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Strings;

namespace PokeSharp.UI;

/// <summary>
/// Simple wrapper around a text value that can be used to retrieve the name of a menu option, either from a fixed
/// value or a value that is generated at runtime.
/// </summary>
public readonly struct HandlerName
{
    private readonly Text _name;
    private readonly Func<Text>? _nameFactory;

    private HandlerName(Text name)
    {
        _name = name;
    }

    private HandlerName(Func<Text> nameFactory)
    {
        _nameFactory = nameFactory;
    }

    public static HandlerName FromDelegate(Func<Text> nameFactory) => new(nameFactory);

    public Text GetName() => _nameFactory?.Invoke() ?? _name;

    public static implicit operator HandlerName(Text name) => new(name);

    public static implicit operator HandlerName(Func<Text> nameFactory) => new(nameFactory);
}

/// <summary>
/// Simple marker type used to indicate that a menu handler condition does not require any additional context.
/// </summary>
public readonly struct NullContext;

public interface IMenuOption
{
    HandlerName Name { get; }

    int? Order { get; }
}

public interface IMenuOption<in TContext> : IMenuOption
{
    Func<TContext, bool>? Condition { get; }
}

public interface IMenuOptionProvider<THandler>
    where THandler : IMenuOption
{
    int Priority { get; }

    IEnumerable<(Name Id, THandler Handler)> GetHandlers();
}

[RegisterSingleton]
public class MenuHandlers<THandler, TContext>(IEnumerable<IMenuOptionProvider<THandler>> providers)
    where THandler : IMenuOption<TContext>
{
    private readonly Dictionary<Name, THandler> _handlers = providers
        .OrderBy(x => x.Priority)
        .SelectMany(x => x.GetHandlers())
        .ToDictionary(x => x.Id, x => x.Handler);

    public IEnumerable<KeyValuePair<Name, THandler>> Handlers => _handlers;

    public IEnumerable<(Name Key, THandler Handler, Text Name)> GetAllAvailable(TContext context)
    {
        foreach (
            var (option, handler) in Handlers.Index().OrderBy(x => x.Item.Value.Order ?? x.Index).Select(x => x.Item)
        )
        {
            if (handler.Condition is not null && !handler.Condition.Invoke(context))
                continue;

            yield return (option, handler, handler.Name.GetName());
        }
    }
}

public static class MenuHandlerExtensions
{
    public static IEnumerable<(Name Key, THandler Handler, Text Name)> GetAllAvailable<THandler>(
        this MenuHandlers<THandler, NullContext> handlers
    )
        where THandler : IMenuOption<NullContext>
    {
        return handlers.GetAllAvailable(new NullContext());
    }
}
