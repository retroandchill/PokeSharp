using Semver;

namespace PokeSharp.Core.Versioning;

public class VersioningService(IGameVersionProvider versionProvider)
{
    // TODO: See if we can bake this into the build, instead of having to remember to bump it
    public SemVersion FrameworkVersion { get; } = new(1, 0, 0);

    public SemVersion GameVersion => versionProvider.GameVersion;
}
