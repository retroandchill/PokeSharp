namespace PokeSharp.Services.DayNightCycle;

public class DayNightService
{
    public static DayNightService Instance { get; } = new();

    public bool IsDay { get; }

    public bool IsNight { get; }

    public bool IsMorning { get; }

    public bool IsAfternoon { get; }

    public bool IsEvening { get; }
}
