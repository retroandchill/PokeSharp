using Injectio.Attributes;
using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Core;
using Semver;

namespace PokeSharp.Saving;

[RegisterSingleton]
[AutoServiceShortcut]
public class SaveGameVersions
{
    public SemVersion PokeSharpVersion { get; set; } = SemVersion.Parse("0.0.0");

    public SemVersion GameVersion { get; set; } = SemVersion.Parse("0.0.0");
}
