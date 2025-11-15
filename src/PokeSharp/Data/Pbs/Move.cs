using System.Collections.Immutable;
using System.Text.Json.Serialization;
using MessagePack;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

/// <summary>
/// The category of damage inflicted by a move.
/// </summary>
public enum DamageCategory : byte
{
    /// <summary>
    /// The move inflicts damage using the user's Attack and the taget's Defense.
    /// </summary>
    Physical,

    /// <summary>
    /// The move inflicts damage using the user's Special Attack and the taget's Special Defense.
    /// </summary>
    Special,

    /// <summary>
    /// The move does not inflict damage.
    /// </summary>
    Status,
}

/// <summary>
/// Represents a move that can be learned by a Pokémon.
/// </summary>
[GameDataEntity(DataPath = "moves")]
[MessagePackObject(true)]
public partial record Move
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// The name of the move, as seen by the player.
    /// </summary>
    public required Text Name { get; init; }

    /// <summary>
    /// The ID of the move's elemental type.
    /// </summary>
    public required Name Type { get; init; }

    /// <summary>
    /// The move's damage category.
    /// </summary>
    public required DamageCategory Category { get; init; }

    /// <summary>
    /// The move's base power value. Moves with a variable base power are defined here with a base power of 1.
    /// For multi-hit moves, this is the base power of a single hit.
    /// </summary>
    public required int Power { get; init; }

    /// <summary>
    /// The move's accuracy, as a percentage (but without the "%" symbol). An accuracy of 0 means the move doesn't
    /// perform an accuracy check (i.e. it will always hit, barring effects like semi-invulnerability).
    /// </summary>
    public required int Accuracy { get; init; }

    /// <summary>
    /// The maximum amount of PP this move can have, not counting modifiers such as the item PP Up.
    /// If the total PP is 0, the move can be used infinitely. Typically, a multiple of 5.
    /// </summary>
    public required int TotalPP { get; init; }

    /// <summary>
    /// The Pokémon that the move will strike
    /// </summary>
    public required Name Target { get; init; }

    /// <summary>
    /// The move's priority, between -6 and 6 inclusive. This is usually 0. A higher priority move will be used before
    /// all moves of lower priority, regardless of speed calculations. Moves with equal priority will be used depending
    /// on which move user is faster.
    /// </summary>
    public required int Priority { get; init; }

    /// <summary>
    /// The move's function code. This is a string of text. Each function code represents a different effect (e.g. poisons the target).
    /// </summary>
    public required Name FunctionCode { get; init; }

    /// <summary>
    /// A comma-separated list of labels applied to the move which can be used to make it behave differently.
    /// </summary>
    public required ImmutableArray<Name> Flags { get; init; }

    /// <summary>
    /// The probability that the move's additional effect occurs, as a percentage (but without the "%" symbol).
    /// If the move has no additional effect (e.g. all status moves), this value is 0.
    /// </summary>
    public required int EffectChance { get; init; }

    /// <summary>
    /// The move's description.
    /// </summary>
    public required Text Description { get; init; }

    /// <summary>
    /// Determines whether the specified flag is present in the list of flags.
    /// </summary>
    /// <param name="flag">The flag to check for in the list of flags.</param>
    /// <returns>
    /// <c>true</c> if the specified flag is present in the list of flags; otherwise, <c>false</c>.
    /// </returns>
    public bool HasFlag(Name flag) => Flags.Contains(flag);

    /// <summary>
    /// Indicates whether the move is Physical or not.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsPhysical => Category == DamageCategory.Physical;

    /// <summary>
    /// Indicates whether the move is Special or not.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsSpecial => Category == DamageCategory.Special;

    /// <summary>
    /// Indicates whether the move is damaging or not.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsDamaging => Category != DamageCategory.Status;

    /// <summary>
    /// Indicates whether the move is a status move or not.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsStatus => Category == DamageCategory.Status;

    /// <summary>
    /// Indicates whether the move is classified as a Hidden Move (HM).
    /// This determination is based on whether the move is associated with any game
    /// data entries marked as Hidden Moves.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsHiddenMove => Item.Entities.Any(i => i.IsHM && i.Move == Id);
}
