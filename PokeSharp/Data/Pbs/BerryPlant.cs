using System.Text.Json.Serialization;
using MessagePack;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

[MessagePackObject(true)]
public readonly record struct YieldRange(int Min, int Max);

[GameDataEntity(DataPath = "berry_plants")]
[MessagePackObject(true)]
public partial record BerryPlant
{
    public required Name Id { get; init; }

    public int HoursPerStage { get; init; } = 3;

    public int DryingPerHour { get; init; } = 15;

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

    [IgnoreMember]
    [JsonIgnore]
    public int MinimumYield => Yield.Min;

    [IgnoreMember]
    [JsonIgnore]
    public int MaximumYield => Yield.Max;
}
