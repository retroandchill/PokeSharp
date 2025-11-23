using PokeSharp.Core.Data;
using UnrealSharp.UnrealSharpCore;

namespace PokeSharp.Unreal.Core.FileSystem;

[RegisterSingleton]
public class UnrealDataFileSource : IDataFileSource
{
    public Stream OpenRead(string path)
    {
        return new UnrealFileSystemStream(CreatePath(path), FileMode.Open);
    }

    public Stream OpenWrite(string path)
    {
        return new FileStream(CreatePath(path), FileMode.Create);
    }

    private static string CreatePath(string path)
    {
        return $"/Game/{path}";
    }
}
