using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using PokeSharp.Abstractions;
using PokeSharp.Data.Core;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

public readonly record struct TrainerIdentifier(Name TrainerType, Text Name, int Version = 0);

public record TrainerPokemon
{
    public required Name Species { get; init; }

    public required int Level { get; init; }

    public int? Form { get; init; }

    public Text? Nickname { get; init; }

    public ImmutableArray<Name>? Moves { get; init; }

    public Name? Ability { get; init; }

    public int? AbilityIndex { get; init; }

    public Name? Item { get; init; }

    public PokemonGender? Gender { get; init; }

    public Name? Nature { get; init; }

    public IReadOnlyDictionary<Name, int>? IVs { get; init; }

    public IReadOnlyDictionary<Name, int>? EVs { get; init; }

    public int? Happiness { get; init; }

    public bool? Shiny { get; init; }

    public bool? SuperShiny { get; init; }

    public bool? Shadow { get; init; }

    public Name? Ball { get; init; }
}

[GameDataEntity(DataPath = "trainers")]
public partial record Trainer
{
    public static bool Exists(Name trainerType, Text name, int version = 0)
    {
        return Exists(new TrainerIdentifier(trainerType, name, version));
    }

    public static Trainer Get(Name trainerType, Text name, int version = 0)
    {
        return Get(new TrainerIdentifier(trainerType, name, version));
    }

    public static bool TryGet(Name trainerType, Text name, [NotNullWhen(true)] out Trainer? trainer)
    {
        return TryGet(new TrainerIdentifier(trainerType, name), out trainer);
    }

    public static bool TryGet(
        Name trainerType,
        Text name,
        int version,
        [NotNullWhen(true)] out Trainer? trainer
    )
    {
        return TryGet(new TrainerIdentifier(trainerType, name, version), out trainer);
    }

    public required TrainerIdentifier Id { get; init; }

    public Name TrainerTypeId => Id.TrainerType;

    public Text Name => Id.Name;

    public int Version => Id.Version;

    public ImmutableArray<Name> Items { get; init; } = [];

    public Text LoseText { get; init; } = TextConstants.Ellipsis;

    public ImmutableArray<TrainerPokemon> Pokemon { get; init; } = [];
}
