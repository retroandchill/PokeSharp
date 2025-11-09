namespace PokeSharp.Core.Data;

public interface IDataLoader
{
    void SaveEntities<T>(IEnumerable<T> entities, string outputPath);

    IEnumerable<T> LoadEntities<T>(string inputPath);
}
