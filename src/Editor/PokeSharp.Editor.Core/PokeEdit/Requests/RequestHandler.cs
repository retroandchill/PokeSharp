using System.Text.Json;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Serialization;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IRequestHandler
{

    void Process<TReader, TWriter>(ref TReader reader, ref TWriter writer, IPokeEditSerializer serializer)
        where TReader : IRequestParameterReader, allows ref struct
        where TWriter : IResponseWriter, allows ref struct;

    ValueTask ProcessAsync(IAsyncRequestParameterReader reader, IAsyncResponseWriter writer,
        IPokeEditSerializer serializer,
        CancellationToken cancellationToken = default);
}
