using Injectio.Attributes;
using Microsoft.Extensions.Options;
using PokeSharp.Core.Versioning;
using Semver;

namespace PokeSharp.Settings;

[RegisterSingleton]
public class DefaultGameVersionProvider(IOptionsMonitor<GameSettings> gameSettings) : IGameVersionProvider
{
    public SemVersion GameVersion => SemVersion.Parse(gameSettings.CurrentValue.Version);
}
