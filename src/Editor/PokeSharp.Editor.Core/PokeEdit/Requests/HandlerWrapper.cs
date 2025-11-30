using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Serialization;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IHandlerWrapper
{
    Name Name { get; }

    void Process(Stream requestStream, Stream responseStream);

    ValueTask ProcessAsync(Stream requestStream, Stream responseStream, CancellationToken cancellationToken = default);
}

public sealed partial class HandlerWrapper<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> handler,
    IPokeEditSerializer serializer
) : IHandlerWrapper
{
    public Name Name => handler.Name;

    [CreateSyncVersion]
    public async ValueTask ProcessAsync(
        Stream requestStream,
        Stream responseStream,
        CancellationToken cancellationToken = default
    )
    {
        var request = await serializer.DeserializeAsync<TRequest>(requestStream, cancellationToken);
        var response = await handler.HandleAsync(request, cancellationToken);
        await serializer.SerializeAsync(responseStream, response, cancellationToken);
    }
}
