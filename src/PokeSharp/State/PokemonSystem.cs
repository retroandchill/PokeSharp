using Injectio.Attributes;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Core;
using PokeSharp.Core.State;

namespace PokeSharp.State;

public enum TextSpeed : byte
{
    Slow = 0,
    Medium = 1,
    Fast = 2,
    Instant = 3,
}

public enum BattleEffect : byte
{
    On,
    Off,
}

public enum BattleStyle : byte
{
    Switch,
    Set,
}

public enum SendToBoxes : byte
{
    Manual,
    Automatic,
}

public enum GiveNicknames : byte
{
    Give,
    DoNotGive,
}

public enum RunStyle : byte
{
    Walk,
    Run,
}

public enum TextInputStyle : byte
{
    Cursor,
    Keyboard,
}

[MessagePackObject(true)]
public class PokemonSystem
{
    public TextSpeed TextSpeed { get; set; } = TextSpeed.Medium;

    public BattleEffect BattleEffect { get; set; } = BattleEffect.On;

    public BattleStyle BattleStyle { get; set; } = BattleStyle.Switch;

    public SendToBoxes SendToBoxes { get; set; } = SendToBoxes.Manual;

    public GiveNicknames GiveNicknames { get; set; } = GiveNicknames.Give;

    public int Frame { get; set; }

    public int TextSkin { get; set; }

    public int Language { get; set; }

    public RunStyle RunStyle { get; set; } = RunStyle.Walk;

    public int BgmVolume { get; set; } = 80;

    public int SeVolume { get; set; } = 100;

    public TextInputStyle TextInputStyle { get; set; } = TextInputStyle.Cursor;
}

public static class PokemonSystemExtensions
{
    private static CachedService<IGameStateAccessor<PokemonSystem>> _cachedService = new();

    [RegisterServices]
    public static void RegisterGameState(this IServiceCollection services)
    {
        services.AddGameState<PokemonSystem>();
    }

    extension(GameGlobal)
    {
        public static PokemonSystem PokemonSystem => _cachedService.Instance.Current;
    }
}
