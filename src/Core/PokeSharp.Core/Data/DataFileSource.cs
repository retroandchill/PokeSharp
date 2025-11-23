namespace PokeSharp.Core.Data;

public interface IDataFileSource
{
    Stream OpenRead(string path);

    Stream OpenWrite(string path);
}
