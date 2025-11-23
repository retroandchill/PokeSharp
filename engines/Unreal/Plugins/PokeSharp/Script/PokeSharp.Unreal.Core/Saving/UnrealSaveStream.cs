using System.Diagnostics.CodeAnalysis;
using PokeSharp.Unreal.Core.Interop;
using UnrealSharp;
using UnrealSharp.Engine;
using UnrealSharp.PokeSharpCore;

namespace PokeSharp.Unreal.Core.Saving;

public sealed class UnrealSaveStream : Stream
{
    private TStrongObjectPtr<UPokeSharpSaveGame>? _strongSaveGame;
    private UPokeSharpSaveGame? SaveGame
    {
        get;
        set
        {
            field = value;
            _strongSaveGame = value;
        }
    }

    private readonly string _saveGameSlotName = "";
    private readonly int _userIndex;

    private int _position;

    private bool _disposed;

    public UnrealSaveStream(UPokeSharpSaveGame saveGame, bool writeMode)
    {
        SaveGame = saveGame;
        CanWrite = writeMode;
        _position = 0;
    }

    public UnrealSaveStream(string saveGameSlotName, int userIndex)
    {
        _saveGameSlotName = saveGameSlotName;
        _userIndex = userIndex;
        CanWrite = false;
    }

    public override bool CanRead => !CanWrite;
    public override bool CanSeek => false;
    public override bool CanWrite { get; }

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => _position;
        set => throw new NotSupportedException();
    }

    public override void Flush()
    {
        // We're reading from a memory buffer, so nothing to flush
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!CanRead)
            throw new IOException("This stream is not readable.");
        EnsureSaveGameInitialized();
        return ReadInternal(SaveGame, buffer, offset, count);
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!CanRead)
            throw new IOException("This stream is not readable.");
        await EnsureSaveGameInitializedAsync();
        return ReadInternal(SaveGame, buffer, offset, count);
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!CanRead)
            throw new IOException("This stream is not readable.");
        await EnsureSaveGameInitializedAsync();

        unsafe
        {
            PokeSharpSaveGameExporter.CallGetDataReadBuffer(
                SaveGame.NativeObject,
                out var readBuffer,
                out var readBufferSize
            );
            var readSpan = new ReadOnlySpan<byte>((byte*)readBuffer, readBufferSize).Slice(_position, buffer.Length);
            _position += readSpan.Length;
            readSpan.CopyTo(buffer.Span);
            return readSpan.Length;
        }
    }

    private unsafe int ReadInternal(UPokeSharpSaveGame saveGame, byte[] buffer, int offset, int count)
    {
        PokeSharpSaveGameExporter.CallGetDataReadBuffer(
            saveGame.NativeObject,
            out var readBuffer,
            out var readBufferSize
        );
        var readSpan = new ReadOnlySpan<byte>((byte*)readBuffer, readBufferSize).Slice(_position, count);
        _position += readSpan.Length;
        var writeSpan = buffer.AsSpan(offset, count);
        readSpan.CopyTo(writeSpan);
        return readSpan.Length;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException("Cannot set seek a save stream.");
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException("Cannot set length of save stream.");
    }

    public override unsafe void Write(byte[] buffer, int offset, int count)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!CanWrite)
            throw new IOException("This stream is not writable.");
        EnsureSaveGameInitialized();

        var writeSpan = buffer.AsSpan(offset, count);
        fixed (byte* writePtr = writeSpan)
        {
            PokeSharpSaveGameExporter.CallWriteToDataBuffer(SaveGame.NativeObject, (IntPtr)writePtr, count);
        }
    }

    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!CanWrite)
            throw new IOException("This stream is not writable.");
        await EnsureSaveGameInitializedAsync();
        WriteInternal(SaveGame, buffer, offset, count);
    }

    public override async ValueTask WriteAsync(
        ReadOnlyMemory<byte> buffer,
        CancellationToken cancellationToken = default
    )
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!CanWrite)
            throw new IOException("This stream is not writable.");
        await EnsureSaveGameInitializedAsync();
        unsafe
        {
            fixed (byte* writePtr = buffer.Span)
            {
                PokeSharpSaveGameExporter.CallWriteToDataBuffer(SaveGame.NativeObject, (IntPtr)writePtr, buffer.Length);
            }
        }
    }

    private static unsafe void WriteInternal(UPokeSharpSaveGame saveGame, byte[] buffer, int offset, int count)
    {
        var writeSpan = buffer.AsSpan(offset, count);
        fixed (byte* writePtr = writeSpan)
        {
            PokeSharpSaveGameExporter.CallWriteToDataBuffer(saveGame.NativeObject, (IntPtr)writePtr, count);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed || !disposing)
            return;

        _strongSaveGame?.Dispose();
        _disposed = true;
    }

    [MemberNotNull(nameof(SaveGame))]
    private void EnsureSaveGameInitialized()
    {
        if (SaveGame is not null)
            return;

        var saveGame = UGameplayStatics.LoadGameFromSlot(_saveGameSlotName, _userIndex);
        if (saveGame is not UPokeSharpSaveGame pokeSharpSaveGame)
        {
            throw new InvalidOperationException("Save game is not a PokeSharp save game.");
        }

        SaveGame = pokeSharpSaveGame;
    }

    [MemberNotNull(nameof(SaveGame))]
    private async ValueTask EnsureSaveGameInitializedAsync()
    {
        if (SaveGame is not null)
            return;

        var saveGame = await UGameplayStatics.LoadGameFromSlotAsync(_saveGameSlotName, _userIndex);
        if (saveGame is not UPokeSharpSaveGame pokeSharpSaveGame)
        {
            throw new InvalidOperationException("Save game is not a PokeSharp save game.");
        }

        SaveGame = pokeSharpSaveGame;
    }
}
