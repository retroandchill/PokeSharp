using PokeSharp.Unreal.Core.Interop;
using UnrealSharp.Core;

namespace PokeSharp.Unreal.Core.FileSystem;

public sealed class UnrealFileSystemStream : Stream
{
    private IntPtr _fileHandle;
    private bool _disposed;
    private readonly FileAccess _access;

    public UnrealFileSystemStream(ReadOnlySpan<char> path, FileMode mode)
        : this(path, mode, mode == FileMode.Append ? FileAccess.Write : FileAccess.ReadWrite) { }

    public UnrealFileSystemStream(ReadOnlySpan<char> path, FileMode fileMode, FileAccess access)
    {
        unsafe
        {
            fixed (char* filenamePtr = path)
            {
                _fileHandle = fileMode switch
                {
                    FileMode.CreateNew => PlatformFileExporter.CallOpenWrite(
                        (IntPtr)filenamePtr,
                        NativeBool.False,
                        access.HasFlag(FileAccess.Read).ToNativeBool(),
                        NativeBool.False
                    ),
                    FileMode.Create => PlatformFileExporter.CallOpenWrite(
                        (IntPtr)filenamePtr,
                        NativeBool.False,
                        access.HasFlag(FileAccess.Read).ToNativeBool(),
                        NativeBool.False
                    ),
                    FileMode.Open => PlatformFileExporter.CallOpenRead(
                        (IntPtr)filenamePtr,
                        access.HasFlag(FileAccess.Write).ToNativeBool()
                    ),
                    FileMode.OpenOrCreate => PlatformFileExporter.CallOpenWrite(
                        (IntPtr)filenamePtr,
                        NativeBool.False,
                        access.HasFlag(FileAccess.Read).ToNativeBool(),
                        NativeBool.False
                    ),
                    FileMode.Truncate => PlatformFileExporter.CallOpenWrite(
                        (IntPtr)filenamePtr,
                        NativeBool.False,
                        access.HasFlag(FileAccess.Read).ToNativeBool(),
                        NativeBool.False
                    ),
                    FileMode.Append => PlatformFileExporter.CallOpenWrite(
                        (IntPtr)filenamePtr,
                        NativeBool.False,
                        access.HasFlag(FileAccess.Read).ToNativeBool(),
                        NativeBool.True
                    ),
                    _ => throw new ArgumentOutOfRangeException(nameof(fileMode), fileMode, null),
                };
            }
        }

        if (_fileHandle == IntPtr.Zero)
            throw new FileNotFoundException("Failed to open file");
        _access = access;
    }

    public override bool CanRead => _access.HasFlag(FileAccess.Read);
    public override bool CanSeek => true;
    public override bool CanWrite => _access.HasFlag(FileAccess.Write);

    public override long Length
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return FileHandleExporter.CallGetSize(_fileHandle);
        }
    }

    public override long Position
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return FileHandleExporter.CallGetPosition(_fileHandle);
        }
        set
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (!FileHandleExporter.CallSeek(_fileHandle, value, SeekOrigin.Begin, out _).ToManagedBool())
            {
                throw new IOException("Failed to seek to position");
            }
        }
    }

    public override void Flush()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!FileHandleExporter.CallFlush(_fileHandle).ToManagedBool())
        {
            throw new IOException("Failed to flush file");
        }
    }

    public override unsafe int Read(byte[] buffer, int offset, int count)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!_access.HasFlag(FileAccess.Read))
            throw new UnauthorizedAccessException("File is not readable");
        var bufferSpan = buffer.AsSpan(offset, count);
        fixed (byte* bufferPtr = bufferSpan)
        {
            return FileHandleExporter.CallRead(_fileHandle, (IntPtr)bufferPtr, count, out var read).ToManagedBool()
                ? read
                : 0;
        }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return FileHandleExporter.CallSeek(_fileHandle, offset, origin, out var newPos).ToManagedBool()
            ? newPos
            : throw new IOException("Failed to seek to position");
    }

    public override void SetLength(long value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!FileHandleExporter.CallSetLength(_fileHandle, value).ToManagedBool())
        {
            throw new IOException("Failed to set length");
        }
    }

    public override unsafe void Write(byte[] buffer, int offset, int count)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!_access.HasFlag(FileAccess.Write))
            throw new UnauthorizedAccessException("File is not writable");
        var bufferSpan = buffer.AsSpan(offset, count);

        fixed (byte* bufferPtr = bufferSpan)
        {
            if (!FileHandleExporter.CallWrite(_fileHandle, (IntPtr)bufferPtr, count).ToManagedBool())
            {
                throw new IOException("Failed to write to file");
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        FileHandleExporter.CallClose(_fileHandle);
        _fileHandle = IntPtr.Zero;
        _disposed = true;
    }
}
