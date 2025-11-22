using PokeSharp.Core;

namespace PokeSharp.Services.DayNightCycle;

[AutoServiceShortcut]
public class DayNightService
{
    public bool IsDay { get; }

    public bool IsNight { get; }

    public bool IsMorning { get; }

    public bool IsAfternoon { get; }

    public bool IsEvening { get; }
}
