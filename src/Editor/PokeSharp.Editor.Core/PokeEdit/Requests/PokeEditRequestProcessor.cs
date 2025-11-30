using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Strings;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

[RegisterSingleton]
[AutoServiceShortcut]
public sealed partial class PokeEditRequestProcessor(IEnumerable<IHandlerWrapper> handlers)
{
    private readonly Dictionary<Name, IHandlerWrapper> _handlers = handlers.ToDictionary(x => x.Name);

    [CreateSyncVersion]
    public async ValueTask ProcessRequestAsync(
        Name requestName,
        Stream requestStream,
        Stream responseStream,
        CancellationToken cancellationToken = default
    )
    {
        if (!_handlers.TryGetValue(requestName, out var handler))
        {
            throw new InvalidOperationException($"Handler for {requestName} not found");
        }

        await handler.ProcessAsync(requestStream, responseStream, cancellationToken);
    }
}
