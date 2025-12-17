using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IAsyncRequestParameterReader
{
    int? ParameterCount { get; }
    int ParameterIndex { get; }

    ValueTask<bool> ReadBooleanAsync(CancellationToken cancellationToken = default);
    ValueTask<byte> ReadByteAsync(CancellationToken cancellationToken = default);
    ValueTask<int> ReadInt32Async(CancellationToken cancellationToken = default);
    ValueTask<long> ReadInt64Async(CancellationToken cancellationToken = default);
    ValueTask<float> ReadSingleAsync(CancellationToken cancellationToken = default);
    ValueTask<double> ReadDoubleAsync(CancellationToken cancellationToken = default);
    ValueTask<Guid> ReadGuidAsync(CancellationToken cancellationToken = default);
    ValueTask<Name> ReadNameAsync(CancellationToken cancellationToken = default);
    
    ValueTask<ReadOnlyMemory<char>> ReadStringAsync(CancellationToken cancellationToken = default);
    ValueTask<ReadOnlyMemory<byte>> ReadBytesAsync(CancellationToken cancellationToken = default);
    
    ValueTask<T> ReadEnumAsync<T>(CancellationToken cancellationToken = default) where T : unmanaged, Enum;
    ValueTask<T> ReadSerializedAsync<T>(CancellationToken cancellationToken = default);
    
    ValueTask SkipAsync(CancellationToken cancellationToken = default);
}