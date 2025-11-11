using System.Collections.Immutable;
using System.Text.Json.Serialization;
using MessagePack;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

[GameDataEntity(DataPath = "shadow_pokemon")]
[MessagePackObject(true)]
public partial record ShadowPokemon
{
    public static ShadowPokemon GetSpeciesForm(Name species, int form) =>
        Get(new SpeciesForm(species, form));

    public required SpeciesForm Id { get; init; }

    [IgnoreMember]
    [JsonIgnore]
    public Name SpeciesId => Id.Species;

    [IgnoreMember]
    [JsonIgnore]
    public int Form => Id.Form;

    public required int GaugeSize { get; init; }

    public required ImmutableArray<Name> Moves { get; init; }

    public required ImmutableArray<Name> Flags { get; init; }

    public bool HasFlag(Name flag) => Flags.Contains(flag);
}
