using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IRequestHandler
{
    void Process(RouteValueBuffer buffer, Stream requestStream, Stream responseStream);

    ValueTask ProcessAsync(
        RouteValueBuffer buffer,
        Stream requestStream,
        Stream responseStream,
        CancellationToken cancellationToken = default
    );
}
