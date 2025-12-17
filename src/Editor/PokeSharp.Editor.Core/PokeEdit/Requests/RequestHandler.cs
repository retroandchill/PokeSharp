using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IRequestHandler
{

    void Process<TReader>(ref TReader reader, IResponseWriter writer)
        where TReader : IRequestParameterReader, allows ref struct;

    ValueTask ProcessAsync(IAsyncRequestParameterReader reader, IAsyncResponseWriter writer,
        CancellationToken cancellationToken = default);
}
