namespace PokeSharp.UI.Pause;

public interface IPauseSceneInfoProvider
{
    int Order { get; }

    bool ShowInfo(PokemonPauseScreen pauseScreen);
}
