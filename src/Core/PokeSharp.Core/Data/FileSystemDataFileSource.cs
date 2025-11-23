using System.IO.Abstractions;

namespace PokeSharp.Core.Data;

public class FileSystemDataFileSource(IFileSystem fileSystem) : IDataFileSource
{
    public Stream OpenRead(string path)
    {
        return fileSystem.File.OpenRead(path);
    }

    public Stream OpenWrite(string path)
    {
        return fileSystem.File.OpenWrite(path);
    }
}
