using System.Collections.Immutable;
using PokeSharp.RGSS.Core;

namespace PokeSharp.RGSS.RPG;

public record Map(int Width, int Height)
{
    public int TilesetId { get; init; } = 1;

    public bool AutoplayBgm { get; init; } = false;

    public AudioFile Bgm { get; init; } = new();

    public bool AutoplayBgs { get; init; } = false;

    public AudioFile Bgs { get; init; } = new();

    public ImmutableArray<int> EncounterList { get; init; } = [];

    public int EncounterStep { get; init; } = 30;

    public Table Data { get; init; } = new(Width, Height, 3);

    public ImmutableDictionary<int, Event> Events { get; init; } = ImmutableDictionary<int, Event>.Empty;
}
