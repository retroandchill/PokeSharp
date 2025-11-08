namespace PokeSharp.Core.Settings;

public static class GameSettings
{
    public const int MaxLevel = 100;
}

public interface IGameSettings<out T>
{
    T Settings { get; }
}