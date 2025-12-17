using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Requests;

public interface IAsyncResponseWriter
{
    ValueTask WriteBooleanAsync(bool value, CancellationToken cancellationToken = default);
    ValueTask WriteByteAsync(byte value, CancellationToken cancellationToken = default);
    ValueTask WriteInt32Async(int value, CancellationToken cancellationToken = default);
    ValueTask WriteInt64Async(long value, CancellationToken cancellationToken = default);
    ValueTask WriteSingleAsync(float value, CancellationToken cancellationToken = default);
    ValueTask WriteDoubleAsync(double value, CancellationToken cancellationToken = default);
    ValueTask WriteGuidAsync(Guid value, CancellationToken cancellationToken = default);
    ValueTask WriteNameAsync(Name value, CancellationToken cancellationToken = default);
    
    ValueTask WriteStringAsync(ReadOnlyMemory<char> value, CancellationToken cancellationToken = default);
    ValueTask WriteBytesAsync(ReadOnlyMemory<byte> value, CancellationToken cancellationToken = default);
    
    ValueTask WriteEnumAsync<T>(T value, CancellationToken cancellationToken = default) where T : unmanaged, Enum;
    ValueTask WriteSerializedAsync<T>(T value, CancellationToken cancellationToken = default);

    ValueTask WriteEmptyAsync(CancellationToken cancellationToken = default);
}