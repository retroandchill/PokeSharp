using System.Collections.Immutable;

namespace PokeSharp.RGSS.RPG;

public record Event(int X, int Y)
{
    public int Id { get; init; } = 0;

    public string Name { get; init; } = "";

    public ImmutableArray<EventPage> Pages { get; init; } = [];
};

public enum MoveType : byte
{
    Fixed = 0,
    Random = 1,
    Approach = 2,
    Custom = 3,
}

public enum MoveSpeed : byte
{
    Slowest = 1,
    Slower = 2,
    Slow = 3,
    Fast = 4,
    Faster = 5,
    Fastest = 6,
}

public enum MoveFrequency : byte
{
    Lowest = 1,
    Lower = 2,
    Low = 3,
    High = 4,
    Higher = 5,
    Highest = 6,
}

public enum FacingDirection : byte
{
    Down = 2,
    Left = 4,
    Right = 6,
    Up = 8,
}

public enum EventTrigger
{
    ActionButton = 0,
    ContactWithPlayer = 1,
    ContactWithEvent = 2,
    Autorun = 3,
    ParallelProcessing = 4,
}

public record EventPage
{
    public EventPageCondition Condition { get; init; } = new();

    public EventPageGraphic Graphic { get; init; } = new();

    public MoveType MoveType { get; init; } = MoveType.Fixed;

    public MoveSpeed MoveSpeed { get; init; } = MoveSpeed.Slow;

    public MoveFrequency MoveFrequency { get; init; } = MoveFrequency.Low;

    public MoveRoute MoveRoute { get; init; } = new();

    public bool WalkAnime { get; init; } = true;

    public bool StepAnime { get; init; } = false;

    public bool DirectionFix { get; init; } = false;

    public bool Through { get; init; } = false;

    public bool AlwaysOnTop { get; init; } = false;

    public EventTrigger Trigger { get; init; } = EventTrigger.ActionButton;

    public ImmutableArray<EventCommand> List { get; init; } = [];
}

public readonly record struct EventCommand(int Code, int Indent, ImmutableArray<object> Parameters)
{
    public EventCommand(int code = 0, int indent = 0)
        : this(code, indent, []) { }
}

public record EventPageGraphic
{
    public int TileId { get; init; } = 0;

    public string CharacterName { get; init; } = "";

    public int CharacterHue { get; init; } = 0;

    public FacingDirection Direction { get; init; } = FacingDirection.Down;

    public int Pattern { get; init; } = 0;

    public int Opacity { get; init; } = 255;

    public int BlendType { get; init; } = 0;
}

public record EventPageCondition
{
    public bool Switch1Valid { get; init; }

    public bool Switch2Valid { get; init; }

    public bool VariableValid { get; init; }

    public bool SelfSwitchValid { get; init; }

    public int Switch1Id { get; init; } = 1;

    public int Switch2Id { get; init; } = 1;

    public int VariableId { get; init; } = 1;

    public int VariableValue { get; init; }

    public char SelfSwitchCh { get; init; } = 'A';
}
