namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IAsyncRequestReader
{
    ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken = default);
}