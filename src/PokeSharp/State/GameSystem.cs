using Injectio.Attributes;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Core;
using PokeSharp.Core.State;

namespace PokeSharp.State;

public enum MessagePosition : byte
{
    Top,
    Center,
    Bottom,
}

[MessagePackObject(true, AllowPrivate = true)]
public partial class GameSystem
{
    public DateTimeOffset? TimerStart { get; set; }
    public int TimerDuration { get; set; }

    public bool SaveDisabled { get; set; }
    public bool MenuDisabled { get; set; }
    public bool EncounterDisabled { get; set; }
    public MessagePosition MessagePosition { get; set; } = MessagePosition.Bottom;
    public int MessageFrame { get; set; }
    public int SaveCount { get; set; }
    public int MagicNumber { get; set; }

    private int _autoScrollXSpeed;
    private int _autoScrollYSpeed;

    public int BgmPosition { get; set; }

    private int _bgsPosition;
}

public static class GameSystemExtensions
{
    private static CachedService<IGameStateAccessor<GameSystem>> _cachedService = new();

    [RegisterServices]
    public static void GameSystem(this IServiceCollection services)
    {
        services.AddGameState<GameSystem>();
    }

    extension(GameServices)
    {
        public static GameSystem GameSystem => _cachedService.Instance.Current;
    }
}
