using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

[RegisterSingleton]
[AutoServiceShortcut]
public sealed class PokeEditRequestProcessor(IEnumerable<IPokeEditController> controllers)
{
    private readonly Dictionary<Name, IPokeEditController> _handlers = controllers.ToDictionary(x => x.Name);

    public void ProcessRequest<TReader>(
        Name controllerName,
        Name methodName,
        ref TReader reader, 
        IResponseWriter writer)
        where TReader : IRequestParameterReader, allows ref struct
    {
        GetHandler(controllerName, methodName).Process(ref reader, writer);
    }
    
    public async ValueTask ProcessRequestAsync(
        Name controllerName,
        Name methodName,
        IAsyncRequestParameterReader reader, 
        IAsyncResponseWriter writer,
        CancellationToken cancellationToken = default
    )
    {
        await GetHandler(controllerName, methodName).ProcessAsync(reader, writer, cancellationToken);
    }

    private IRequestHandler GetHandler(Name controllerName, Name methodName)
    {
        if (!_handlers.TryGetValue(controllerName, out var controller))
        {
            throw new InvalidOperationException($"Controller with name '{controllerName}' not found");
        }

        return controller.GetRequestHandler(methodName) ?? throw new InvalidOperationException($"Handler for '{methodName}' on '{controllerName}' not found");
    }
}
