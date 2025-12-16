using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IRequestHandler
{
    Name Name { get; }

    void Process(Stream requestStream, Stream responseStream);

    void Process<TReader>(ref TReader reader, IRequestWriter writer)
        where TReader : IRequestReader, allows ref struct
    {
        // TODO: Remove the default implementation once migration is done. This is currently just ensure that the library still compiles.
    }

    ValueTask ProcessAsync(Stream requestStream, Stream responseStream, CancellationToken cancellationToken = default);

    ValueTask ProcessAsync(IAsyncRequestParameterReader reader, IAsyncRequestWriter writer,
        CancellationToken cancellationToken = default)
    {
        
        // TODO: Remove the default implementation once migration is done. This is currently just ensure that the library still compiles.
        return ValueTask.CompletedTask;
    }
}
