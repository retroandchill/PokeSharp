namespace PokeSharp.RGSS.RPG;

public record MapInfo
{
    public string Name { get; init; } = "";

    public int ParentId { get; init; }

    public int Order { get; init; }

    public bool Expanded { get; init; }

    public int ScrollX { get; init; }

    public int ScrollY { get; init; }
}
