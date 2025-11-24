using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using MessagePack;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using PokeSharp.Core.Strings;

namespace PokeSharp.Data.Pbs;

/// <summary>
/// Represents data about enemy Shadow Pokémon that can be caught by the player.
/// </summary>
[GameDataEntity(DataPath = "shadow_pokemon")]
[MessagePackObject(true)]
public partial record ShadowPokemon
{
    /// <summary>
    /// Get data for a specific species and form.
    /// </summary>
    /// <param name="species">The primary ID of the species.</param>
    /// <param name="form">The specific form to search for.</param>
    /// <returns></returns>
    public static ShadowPokemon Get(Name species, int form) => Get(new SpeciesForm(species, form));

    public static bool TryGet(Name species, [NotNullWhen(true)] out ShadowPokemon? shadowPokemon)
    {
        return TryGet(new SpeciesForm(species), out shadowPokemon);
    }

    public static bool TryGet(Name species, int form, [NotNullWhen(true)] out ShadowPokemon? shadowPokemon)
    {
        return TryGet(new SpeciesForm(species, form), out shadowPokemon);
    }

    public const int MaxGaugeSize = 4000;

    /// <inheritdoc />
    public required SpeciesForm Id { get; init; }

    /// <summary>
    /// The ID of the individual species.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public Name SpeciesId => Id.Species;

    /// <summary>
    /// The form for this info.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public int Form => Id.Form;

    /// <summary>
    /// The size of the Pokémon's Heart Gauge. The larger this number, the harder a Pokémon is to purify.
    /// </summary>
    public required int GaugeSize { get; init; }

    /// <summary>
    /// The Shadow Moves known by the Shadow Pokémon. These replace the actual known moves until the Pokémon is purified.
    /// </summary>
    public required ImmutableArray<Name> Moves { get; init; }

    /// <summary>
    /// Comma-separated labels applied to the shadow Pokémon which can be used to make it behave differently.
    /// </summary>
    public required ImmutableArray<Name> Flags { get; init; }

    /// <summary>
    /// Determines whether the specified flag is present in the list of flags.
    /// </summary>
    /// <param name="flag">The flag to check for in the list of flags.</param>
    /// <returns>
    /// <c>true</c> if the specified flag is present in the list of flags; otherwise, <c>false</c>.
    /// </returns>
    public bool HasFlag(Name flag) => Flags.Contains(flag);
}
