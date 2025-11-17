using System.Collections.Immutable;

namespace PokeSharp.RGSS.RPG;

public record MoveRoute
{
    public bool Repeat { get; init; } = true;

    public bool Skippable { get; init; } = false;

    public ImmutableArray<MoveCommand> List { get; init; } = [new()];
}

public readonly record struct MoveCommand(int Code, ImmutableArray<int> Parameters)
{
    public MoveCommand(int code = 0)
        : this(code, []) { }
}
