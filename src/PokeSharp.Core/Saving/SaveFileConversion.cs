using Semver;

namespace PokeSharp.Core.Saving;

public enum  ConversionTriggerType : byte
{
    Framework,
    Game,
}

public interface ISaveDataConversion
{
    Name Id { get; }

    Text Title { get; }

    ConversionTriggerType TriggerType { get; }

    SemVersion Version { get; }

    bool ShouldRun(SemVersion version);

    void Run(Dictionary<Name, object> saveData);

    void RunSingle(object value, Name key);
}
