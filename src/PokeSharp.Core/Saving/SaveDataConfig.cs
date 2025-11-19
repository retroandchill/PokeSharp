namespace PokeSharp.Core.Saving;

public record SaveDataConfig
{
    public string SaveFileName { get; init; } = "Game.sav";
    
    public string SaveFilePath { get; init; } = "Saves";
}
