using System.Diagnostics.CodeAnalysis;
using MessagePack;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using PokeSharp.Core.Strings;

namespace PokeSharp.Data.Pbs;

[GameDataEntity(DataPath = "species_metrics")]
[MessagePackObject(true)]
public partial record SpeciesMetrics
{
    public required SpeciesForm Id { get; init; }

    public required Point BackSprite { get; init; }

    public required Point FrontSprite { get; init; }

    public required int FrontSpriteAltitude { get; init; }

    public required int ShadowX { get; init; }

    public required int ShadowSize { get; init; }

    // ReSharper disable once MemberCanBeMadeStatic.Global
#pragma warning disable CA1822
    public bool ShowsShadow => true;
#pragma warning restore CA1822

    public static SpeciesMetrics Get(Name species, int form = 0) => Get(new SpeciesForm(species, form));

    public static bool TryGet(Name species, [NotNullWhen(true)] out SpeciesMetrics? result)
    {
        return TryGet(new SpeciesForm(species), out result);
    }

    public static bool TryGet(Name species, int form, [NotNullWhen(true)] out SpeciesMetrics? result)
    {
        return TryGet(new SpeciesForm(species, form), out result);
    }
}
