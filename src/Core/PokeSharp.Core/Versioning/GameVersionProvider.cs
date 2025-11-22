using Semver;

namespace PokeSharp.Core.Versioning;

public interface IGameVersionProvider
{
    SemVersion GameVersion { get; }
}
