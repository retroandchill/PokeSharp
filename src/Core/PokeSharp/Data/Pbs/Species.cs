using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using MessagePack;
using PokeSharp.Core.Data;
using PokeSharp.Core.Strings;
using PokeSharp.Serialization.Json;
using PokeSharp.Serialization.MessagePack;

namespace PokeSharp.Data.Pbs;

[MessagePackObject(true)]
public readonly record struct SpeciesForm(Name Species, int Form = 0)
{
    public static implicit operator SpeciesForm(Name species) => new(species);

    public static implicit operator SpeciesForm(string species) => new(species);

    public override string ToString() => Form > 0 ? $"{Species},{Form}" : Species.ToString();
}

[MessagePackObject(true)]
public readonly record struct LevelUpMove(Name Move, int Level);

public enum EvolutionParameterType : byte
{
    Null,
    Int,
    Name
}

[MessagePackFormatter(typeof(EvolutionParameterFormatter))]
[JsonConverter(typeof(EvolutionParameterJsonConverter))]
public readonly struct EvolutionParameter : IEquatable<EvolutionParameter>, IEquatable<int>, IEquatable<Name>, IEqualityOperators<EvolutionParameter, EvolutionParameter, bool>, IEqualityOperators<EvolutionParameter, int, bool>, IEqualityOperators<EvolutionParameter, Name, bool>
{
    [StructLayout(LayoutKind.Explicit)]
    private readonly struct Storage
    {
        [FieldOffset(0)]
        public readonly int IntValue;

        [FieldOffset(0)] 
        public readonly Name NameValue;

        public Storage(int intValue)
        {
            IntValue = intValue;
        }

        public Storage(Name nameValue)
        {
            NameValue = nameValue;
        }
    }
    
    public EvolutionParameterType Kind { get; }
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    private readonly Storage _value;

    private EvolutionParameter(EvolutionParameterType kind, Storage value)
    {
        Kind = kind;
        _value = value;
    }
    
    public EvolutionParameter(int value) : this(EvolutionParameterType.Int, new Storage(value)) { }
    
    public EvolutionParameter(Name value) : this(EvolutionParameterType.Name, new Storage(value)) { }

    public static EvolutionParameter Null => new();
    
    public static implicit operator EvolutionParameter(int value) => new(value);
    
    public static implicit operator EvolutionParameter(Name value) => new(value);
    
    public bool IsNull => Kind == EvolutionParameterType.Null;
    
    public bool IsInt => Kind == EvolutionParameterType.Int;
    
    public bool IsName => Kind == EvolutionParameterType.Name;

    public int GetInteger()
    {
        return IsInt ? _value.IntValue : throw new InvalidOperationException("Value is not an integer");
    }

    public bool TryGetInteger(out int value)
    {
        if (IsInt)
        {
            value = _value.IntValue;
            return true;
        }

        value = 0;
        return false;
    }
    
    public Name GetName()
    {
        return IsName ? _value.NameValue : throw new InvalidOperationException("Value is not a name");
    }

    public bool TryGetName(out Name name)
    {
        if (IsName)
        {
            name = _value.NameValue;
            return true;
        }

        name = Name.None;
        return false;
    }

    public void Match(Action<int> whenInt, Action<Name> whenName, Action whenNull)
    {
        switch (Kind)
        {
            case EvolutionParameterType.Null:
                whenNull();
                break;
            case EvolutionParameterType.Int:
                whenInt(_value.IntValue);
                break;
            case EvolutionParameterType.Name:
                whenName(_value.NameValue);
                break;
            default:
                throw new InvalidOperationException("Invalid EvolutionParameterType");
        }
    }
    
    public T Match<T>(Func<int, T> whenInt, Func<Name, T> whenName, Func<T> whenNull)
    {
        return Kind switch
        {
            EvolutionParameterType.Null => whenNull(),
            EvolutionParameterType.Int => whenInt(_value.IntValue),
            EvolutionParameterType.Name => whenName(_value.NameValue),
            _ => throw new InvalidOperationException("Invalid EvolutionParameterType")
        };
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is EvolutionParameter other && this == other;
    }

    public bool Equals(EvolutionParameter other)
    {
        return this == other;
    }

    public bool Equals(int other)
    {
        return this == other;
    }

    public bool Equals(Name other)
    {
        return this == other;
    }

    public static bool operator ==(EvolutionParameter left, EvolutionParameter right)
    {
        return left.Kind switch
        {
            EvolutionParameterType.Null => right.IsNull,
            EvolutionParameterType.Int => right.IsInt && left._value.IntValue == right._value.IntValue,
            EvolutionParameterType.Name => right.IsName && left._value.NameValue == right._value.NameValue,
            _ => throw new InvalidOperationException("Invalid EvolutionParameterType")
        };
    }

    public static bool operator !=(EvolutionParameter left, EvolutionParameter right)
    {
        return !(left == right);
    }

    public static bool operator ==(EvolutionParameter left, int right)
    {
        return left.TryGetInteger(out var value) && value == right;
    }

    public static bool operator !=(EvolutionParameter left, int right)
    {
        return !(left == right);
    }

    public static bool operator ==(EvolutionParameter left, Name right)
    {
        return left.TryGetName(out var name) && name == right;
    }

    public static bool operator !=(EvolutionParameter left, Name right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return Match(i => i.ToString(), n =>  n.ToString(), () => "null");
    }

    public override int GetHashCode()
    {
        return Match(i => HashCode.Combine(EvolutionParameterType.Int, i),
            n => HashCode.Combine(EvolutionParameterType.Name, n),
            () => EvolutionParameterType.Null.GetHashCode());
    }
}

[MessagePackObject(true)]
public record EvolutionInfo(
    Name Species,
    Name EvolutionMethod,
    EvolutionParameter Parameter = default,
    bool IsPrevious = false
);

public readonly record struct EvolutionFamily(
    Name PreviousSpecies,
    Name Species,
    Name EvolutionMethod,
    object? Parameter = null,
    bool IsPrevious = false
);

public enum MegaMessageType
{
    Normal,
    Move,
}

[GameDataEntity(DataPath = "species")]
[MessagePackObject(true)]
public partial record Species
{
    public static Species Get(Name species, int form = 0)
    {
        return Get(new SpeciesForm(species, form));
    }

    public static bool TryGet(Name species, [NotNullWhen(true)] out Species? result)
    {
        return TryGet(new SpeciesForm(species), out result);
    }

    public static bool TryGet(Name species, int form, [NotNullWhen(true)] out Species? result)
    {
        return TryGet(new SpeciesForm(species, form), out result);
    }

    public static IEnumerable<Species> AllSpecies => Entities.Where(e => e.Form == 0);

    public static int SpeciesCount => AllSpecies.Count();

    public required SpeciesForm Id { get; init; }

    [IgnoreMember]
    public Name SpeciesId => Id.Species;

    [IgnoreMember]
    public int Form => Id.Form;

    public required Text Name { get; init; }

    public required Text? FormName { get; init; }

    public required Text Category { get; init; }

    public required Text PokedexEntry { get; init; }

    private readonly int? _pokedexForm;

    public int PokedexForm
    {
        get => _pokedexForm ?? Form;
        init => _pokedexForm = value;
    }

    public required ImmutableArray<Name> Types { get; init; }

    public required ImmutableDictionary<Name, int> BaseStats { get; init; }

    public required ImmutableDictionary<Name, int> EVs { get; init; }

    public required int BaseExp { get; init; }

    public required Name GrowthRate { get; init; }

    public required Name GenderRatio { get; init; }

    public required int CatchRate { get; init; }

    public required int Happiness { get; init; }

    public required ImmutableArray<LevelUpMove> LevelUpMoves { get; init; }

    public required ImmutableArray<Name> TutorMoves { get; init; }

    public required ImmutableArray<Name> EggMoves { get; init; }

    public required ImmutableArray<Name> Abilities { get; init; }

    public required ImmutableArray<Name> HiddenAbilities { get; init; }

    public required ImmutableArray<Name> WildItemCommon { get; init; }

    public required ImmutableArray<Name> WildItemUncommon { get; init; }

    public required ImmutableArray<Name> WildItemRare { get; init; }

    public required ImmutableArray<Name> EggGroups { get; init; }

    public required int HatchSteps { get; init; }

    public required Name Incense { get; init; }

    public required ImmutableArray<Name> Offspring { get; init; }

    public required ImmutableArray<EvolutionInfo> Evolutions { get; init; }

    public required int Height { get; init; }

    public required int Weight { get; init; }

    public required Name Color { get; init; }

    public required Name Shape { get; init; }

    public required Name Habitat { get; init; }

    public required int Generation { get; init; }

    public required ImmutableArray<Name> Flags { get; init; }

    public Name MegaStone { get; init; }

    public Name MegaMove { get; init; }

    public int UnmegaForm { get; init; }

    public MegaMessageType MegaMessage { get; init; }

    [IgnoreMember]
    [JsonIgnore]
    public int? DefaultForm
    {
        get
        {
            foreach (var match in Flags.Select(flag => DefaultFormPattern.Match(flag)).Where(match => match.Success))
            {
                return int.Parse(match.Groups[1].Value);
            }

            return null;
        }
    }

    [IgnoreMember]
    [JsonIgnore]
    public int BaseForm => DefaultForm ?? Form;

    [IgnoreMember]
    [JsonIgnore]
    public bool IsSingleGendered => Core.GenderRatio.Get(GenderRatio).IsSingleGender;

    [IgnoreMember]
    [JsonIgnore]
    public int BaseStatTotal => BaseStats.Values.Sum();

    /// <summary>
    /// Determines whether the specified flag is present in the list of flags.
    /// </summary>
    /// <param name="flag">The flag to check for in the list of flags.</param>
    /// <returns>
    /// <c>true</c> if the specified flag is present in the list of flags; otherwise, <c>false</c>.
    /// </returns>
    public bool HasFlag(Name flag) => Flags.Contains(flag);

    [GeneratedRegex("DefaultForm_(\\d+)")]
    private static partial Regex DefaultFormPattern { get; }
}
