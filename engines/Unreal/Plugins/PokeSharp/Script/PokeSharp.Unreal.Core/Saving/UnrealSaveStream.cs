using PokeSharp.Unreal.Core.Interop;
using UnrealSharp;
using UnrealSharp.PokeSharpCore;

namespace PokeSharp.Unreal.Core.Saving;

public sealed class UnrealSaveStream(UPokeSharpSaveGame saveGame, bool writeMode) : Stream
{
    private readonly TStrongObjectPtr<UPokeSharpSaveGame> _saveGame = saveGame;
    private bool _disposed;

    public override bool CanRead => !writeMode;
    public override bool CanSeek => CanRead;
    public override bool CanWrite => writeMode;

    public override long Length
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return _saveGame.Value?.DataSize ?? throw new InvalidOperationException("Save game is not loaded.");
        }
    }

    public override long Position
    {
        get;
        set
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!CanRead)
                throw new IOException("This stream is not readable.");
            field = Math.Clamp(value, 0, Length - 1);
        }
    }

    public override void Flush()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        // We're reading from a memory buffer, so nothing to flush
    }

    public override unsafe int Read(byte[] buffer, int offset, int count)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!CanRead)
            throw new IOException("This stream is not readable.");
        if (_saveGame.Value is null)
            throw new InvalidOperationException("Save game is not loaded.");

        PokeSharpSaveGameExporter.CallGetDataReadBuffer(
            _saveGame.Value.NativeObject,
            out var readBuffer,
            out var readBufferSize
        );
        var readSpan = new ReadOnlySpan<byte>((byte*)readBuffer, readBufferSize).Slice((int)Position, count);
        Position += readSpan.Length;
        var writeSpan = buffer.AsSpan(offset, count);
        readSpan.CopyTo(writeSpan);
        return readSpan.Length;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!CanSeek)
            throw new IOException("This stream is not seekable.");
        Position = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => Position + offset,
            SeekOrigin.End => Length - offset - 1,
            _ => throw new ArgumentOutOfRangeException(nameof(origin), origin, null),
        };

        return Position;
    }

    public override void SetLength(long value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        throw new NotSupportedException("Cannot set length of save stream.");
    }

    public override unsafe void Write(byte[] buffer, int offset, int count)
    {
        if (!CanWrite)
            throw new IOException("This stream is not writable.");
        if (_saveGame.Value is null)
            throw new InvalidOperationException("Save game is not loaded.");

        var writeSpan = buffer.AsSpan(offset, count);
        fixed (byte* writePtr = writeSpan)
        {
            PokeSharpSaveGameExporter.CallWriteToDataBuffer(_saveGame.Value.NativeObject, (IntPtr)writePtr, count);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed || !disposing)
            return;

        _saveGame.Dispose();
        _disposed = true;
    }
}
