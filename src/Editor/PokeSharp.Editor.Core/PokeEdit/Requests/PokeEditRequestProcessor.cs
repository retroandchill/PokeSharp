using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Strings;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

[RegisterSingleton]
[AutoServiceShortcut]
public sealed partial class PokeEditRequestProcessor(IEnumerable<IRequestHandler> handlers)
{
    private readonly Dictionary<Name, IRequestHandler> _handlers = handlers.ToDictionary(x => x.Name);

    public void ProcessRequest(Name requestName, Stream requestStream, Stream responseStream)
    {
        if (!_handlers.TryGetValue(requestName, out var handler))
        {
            throw new InvalidOperationException($"Handler for {requestName} not found");
        }

        handler.Process(requestStream, responseStream);
    }
}
