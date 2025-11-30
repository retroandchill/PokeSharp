using PokeSharp.Unreal.Core.Interop;
using PokeSharp.Unreal.Core.Utils;
using UnrealSharp.Core;

namespace PokeSharp.Unreal.Core.Serialization;

public sealed class ArchiveStream : Stream
{
    private const int IndexNone = -1;

    private SharedPtr _archive;
    private bool _disposed;

    public ArchiveStream(ref SharedPtr archive)
    {
        ArchiveStreamExporter.CallCopy(ref _archive, ref archive);
    }

    ~ArchiveStream()
    {
        Dispose(false);
    }

    public override bool CanRead
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return ArchiveStreamExporter.CallCanRead(_archive.Pointer).ToManagedBool();
        }
    }

    public override bool CanSeek
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return ArchiveStreamExporter.CallCanSeek(_archive.Pointer).ToManagedBool();
        }
    }

    public override bool CanWrite
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            return ArchiveStreamExporter.CallCanWrite(_archive.Pointer).ToManagedBool();
        }
    }

    public override long Length
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var length = ArchiveStreamExporter.CallGetLength(_archive.Pointer);
            return length != IndexNone
                ? length
                : throw new NotSupportedException("Cannot get length of archive stream");
        }
    }

    public override long Position
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var position = ArchiveStreamExporter.CallGetPosition(_archive.Pointer);
            return position != IndexNone
                ? position
                : throw new NotSupportedException("Cannot get position of archive stream");
        }
        set => Seek(value, SeekOrigin.Begin);
    }

    public override void Flush()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArchiveStreamExporter.CallFlush(_archive.Pointer);
    }

    public override unsafe int Read(byte[] buffer, int offset, int count)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var bufferSpan = buffer.AsSpan(offset, count);
        fixed (byte* bufferPtr = bufferSpan)
        {
            return ArchiveStreamExporter
                .CallRead(_archive.Pointer, (IntPtr)bufferPtr, bufferSpan.Length, out var read)
                .ToManagedBool()
                ? read
                : throw new NotSupportedException("Cannot read from archive stream");
        }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return ArchiveStreamExporter.CallSeek(_archive.Pointer, offset, origin, out var newPosition).ToManagedBool()
            ? newPosition
            : throw new NotSupportedException("Cannot seek archive stream");
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException("Cannot set length of archive stream");
    }

    public override unsafe void Write(byte[] buffer, int offset, int count)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var bufferSpan = buffer.AsSpan(offset, count);
        fixed (byte* bufferPtr = bufferSpan)
        {
            if (
                !ArchiveStreamExporter.CallWrite(_archive.Pointer, (IntPtr)bufferPtr, bufferSpan.Length).ToManagedBool()
            )
            {
                throw new NotSupportedException("Cannot write to archive stream");
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        _disposed = true;
        ArchiveStreamExporter.CallRelease(ref _archive);
    }
}
