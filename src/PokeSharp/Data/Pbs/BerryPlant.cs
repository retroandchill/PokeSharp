using System.Text.Json.Serialization;
using MessagePack;
using PokeSharp.Core;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

/// <summary>
/// Represents a range of yields typically associated with a berry plant, defined by a minimum and maximum value.
/// </summary>
/// <param name="Min">The minimum yield value.</param>
/// <param name="Max">The maximum yield value.</param>
[MessagePackObject(true)]
public readonly record struct YieldRange(int Min, int Max);

/// <summary>
/// Represents a berry plant entity with properties associated with its growth, yield, and drying characteristics.
/// </summary>
[GameDataEntity(DataPath = "berry_plants")]
[MessagePackObject(true)]
public partial record BerryPlant
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// The growth rate, i.e. the number of hours between growth stages.
    /// </summary>
    public int HoursPerStage { get; init; } = 3;

    /// <summary>
    /// How much moisture the soil loses per hour (newer berry plant mechanics only). A berry plant begins with 100
    /// moisture, and returns to 100 moisture when it is watered.
    /// </summary>
    public int DryingPerHour { get; init; } = 15;

    /// <summary>
    /// Two numbers that denote a range. The number of berries yielded by the berry plant falls within this range,
    /// and is higher the more it was watered.
    /// </summary>
    public YieldRange Yield
    {
        get;
        init
        {
            if (value.Min > value.Max)
            {
                field = new YieldRange(value.Max, value.Min);
            }

            field = value;
        }
    } = new(2, 5);

    /// <summary>
    /// The minimum yield value.
    /// </summary>
    /// <seealso cref="BerryPlant.Yield"/>
    [IgnoreMember]
    [JsonIgnore]
    public int MinimumYield => Yield.Min;

    /// <summary>
    /// The maximum yield value.
    /// </summary>
    /// <seealso cref="BerryPlant.Yield"/>
    [IgnoreMember]
    [JsonIgnore]
    public int MaximumYield => Yield.Max;
}
