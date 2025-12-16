namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IAsyncRequestWriter
{
    ValueTask WriteAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default);
    
    ValueTask FlushAsync(CancellationToken cancellationToken = default);
}