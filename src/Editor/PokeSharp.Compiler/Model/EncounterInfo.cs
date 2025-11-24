using PokeSharp.Core;
using PokeSharp.Core.Strings;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Model;

public class EncounterInfo
{
    public required EncounterId Id { get; init; }

    public int Map => Id.MapId;

    public int Version => Id.Version;

    public Dictionary<Name, int> StepChances { get; set; } = new();

    public Dictionary<Name, List<EncounterEntry>> Types { get; set; } = new();
}
