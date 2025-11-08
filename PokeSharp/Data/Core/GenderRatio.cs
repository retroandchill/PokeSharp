using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Core;

public enum PokemonGender : byte
{
    /// <summary>
    /// Male gender.
    /// </summary>
    Male = 0,

    /// <summary>
    /// Female gender.
    /// </summary>
    Female = 1,

    /// <summary>
    /// Gender unknown.
    /// </summary>
    Genderless = 2,
}

public enum SpecialGenderRatio : byte
{
    /// <summary>
    /// No special gender ratio.
    /// </summary>
    None,

    /// <summary>
    /// All Pokémon are exclusively male.
    /// </summary>
    MaleOnly,

    /// <summary>
    /// All Pokémon are exclusively female.
    /// </summary>
    FemaleOnly,

    /// <summary>
    /// All Pokémon are genderless.
    /// </summary>
    GenderlessOnly,
}

public readonly struct GenderRatioData
{
    private readonly SpecialGenderRatio _specialGenderRatio;

    private readonly byte _femaleChance;

    private GenderRatioData(PokemonGender gender)
    {
        _specialGenderRatio = gender switch
        {
            PokemonGender.Male => SpecialGenderRatio.MaleOnly,
            PokemonGender.Female => SpecialGenderRatio.FemaleOnly,
            PokemonGender.Genderless => SpecialGenderRatio.GenderlessOnly,
            _ => throw new ArgumentOutOfRangeException(nameof(gender), gender, null),
        };
    }

    private GenderRatioData(byte femaleChance)
    {
        _specialGenderRatio = SpecialGenderRatio.None;
        _femaleChance = femaleChance;
    }

    /// <summary>
    /// Creates a gender ratio data instance representing a single-gender Pokémon.
    /// </summary>
    /// <param name="gender">The gender of the Pokémon. Can be Male, Female, or Genderless as specified by the EPokemonGender enumeration.</param>
    /// <returns>A gender ratio data instance for the specified single gender.</returns>
    public static GenderRatioData SingleGender(PokemonGender gender) => new(gender);

    /// <summary>
    /// Creates a gender ratio data instance representing a specific chance for the Pokémon to be female.
    /// </summary>
    /// <param name="chance">The percentage chance for the Pokémon to be female, represented as a byte in the range of 0 to 255.</param>
    /// <returns>A gender ratio data instance with the specified female chance.</returns>
    public static GenderRatioData FemaleChance(byte chance) => new(chance);

    /// <summary>
    /// Implicitly converts an <see cref="PokemonGender"/> value to an instance of <see cref="GenderRatioData"/>.
    /// </summary>
    /// <param name="gender">The gender of the Pokémon to convert. Must be a value from the <see cref="PokemonGender"/> enumeration.</param>
    /// <returns>
    /// A new <see cref="GenderRatioData"/> instance representing the specified single gender.
    /// </returns>
    public static implicit operator GenderRatioData(PokemonGender gender) => SingleGender(gender);

    /// <summary>
    /// Indicates whether the associated entity is confined to a single gender.
    /// This property evaluates the gender ratio data and determines if the entity
    /// exclusively represents one gender (e.g., male-only, female-only, or genderless-only).
    /// </summary>
    public bool IsSingleGender => _specialGenderRatio != SpecialGenderRatio.None;

    /// <summary>
    /// Applies specific operations based on the gender ratio data type.
    /// </summary>
    /// <param name="onFemaleChance">The action to execute if the gender ratio data represents a female chance, providing the chance as a byte parameter.</param>
    /// <param name="onSingleGender">The action to execute if the gender ratio data represents a single-gender Pokémon, providing the gender as an EPokemonGender parameter.</param>
    public void Match(Action<byte> onFemaleChance, Action<PokemonGender> onSingleGender)
    {
        switch (_specialGenderRatio)
        {
            case SpecialGenderRatio.None:
                onFemaleChance(_femaleChance);
                break;
            case SpecialGenderRatio.MaleOnly:
                onSingleGender(PokemonGender.Male);
                break;
            case SpecialGenderRatio.FemaleOnly:
                onSingleGender(PokemonGender.Female);
                break;
            case SpecialGenderRatio.GenderlessOnly:
                onSingleGender(PokemonGender.Genderless);
                break;
            default:
                throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Matches the gender ratio data with the specified logic for female chance or single-gender scenarios.
    /// </summary>
    /// <param name="onFemaleChance">A function to handle cases where the gender is determined by a female chance value.</param>
    /// <param name="onSingleGender">A function to handle cases where the Pokémon has a single-gender specification.</param>
    /// <returns>The result of executing the matching logic based on the provided functions.</returns>
    public T Match<T>(Func<byte, T> onFemaleChance, Func<PokemonGender, T> onSingleGender)
    {
        return _specialGenderRatio switch
        {
            SpecialGenderRatio.None => onFemaleChance(_femaleChance),
            SpecialGenderRatio.MaleOnly => onSingleGender(PokemonGender.Male),
            SpecialGenderRatio.FemaleOnly => onSingleGender(PokemonGender.Female),
            SpecialGenderRatio.GenderlessOnly => onSingleGender(PokemonGender.Genderless),
            _ => throw new InvalidOperationException(),
        };
    }
}

[GameDataEntity]
public readonly partial record struct GenderRatio
{
    public required Name Id { get; init; }
    
    public required Text Name { get; init; }
    
    public required GenderRatioData Ratio { get; init; }
    
    public bool IsSingleGender => Ratio.IsSingleGender;
    
    #region Defaults
    private const string LocalizationNamespace = "GameData.GenderRatio";

    public static void AddDefaultValues()
    {
        Register(
            new GenderRatio
            {
                Id = "AlwaysMale",
                Name = Text.Localized(LocalizationNamespace, "AlwaysMale", "Always Male"),
                Ratio = PokemonGender.Male,
            }
        );

        Register(
            new GenderRatio
            {
                Id = "AlwaysFemale",
                Name = Text.Localized(LocalizationNamespace, "AlwaysFemale", "Always Female"),
                Ratio = PokemonGender.Female,
            }
        );

        Register(
            new GenderRatio
            {
                Id = "Genderless",
                Name = Text.Localized(LocalizationNamespace, "Genderless", "Genderless"),
                Ratio = PokemonGender.Genderless,
            }
        );

        Register(
            new GenderRatio
            {
                Id = "FemaleOneEighth",
                Name = Text.Localized(LocalizationNamespace, "FemaleOneEighth", "Female One Eighth"),
                Ratio = GenderRatioData.FemaleChance(32),
            }
        );

        Register(
            new GenderRatio
            {
                Id = "Female25Percent",
                Name = Text.Localized(LocalizationNamespace, "Female25Percent", "Female 25 Percent"),
                Ratio = GenderRatioData.FemaleChance(64),
            }
        );

        Register(
            new GenderRatio
            {
                Id = "Female50Percent",
                Name = Text.Localized(LocalizationNamespace, "Female50Percent", "Female 50 Percent"),
                Ratio = GenderRatioData.FemaleChance(128),
            }
        );

        Register(
            new GenderRatio
            {
                Id = "Female75Percent",
                Name = Text.Localized(LocalizationNamespace, "Female75Percent", "Female 75 Percent"),
                Ratio = GenderRatioData.FemaleChance(192),
            }
        );

        Register(
            new GenderRatio
            {
                Id = "FemaleSevenEighths",
                Name = Text.Localized(LocalizationNamespace, "FemaleSevenEighths", "Female Seven Eighths"),
                Ratio = GenderRatioData.FemaleChance(224),
            }
        );
    }

    #endregion
}