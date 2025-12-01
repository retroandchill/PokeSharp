using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IRequestHandler
{
    Name Name { get; }

    void Process(Stream requestStream, Stream responseStream);

    ValueTask ProcessAsync(Stream requestStream, Stream responseStream, CancellationToken cancellationToken = default);
}
