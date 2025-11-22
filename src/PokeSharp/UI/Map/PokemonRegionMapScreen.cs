using PokeSharp.Data.Pbs;

namespace PokeSharp.UI.Map;

public class PokemonRegionMapScreen(IPokemonRegionMapScene scene)
{
    public async ValueTask<FlyDestination?> StartFlyScreen()
    {
        using var scope = scene.StartScene(false, true);
        var result = await scene.MapScene();
        return result;
    }

    public async ValueTask<FlyDestination?> StartScreen()
    {
        using var scope = scene.StartScene();
        var result = await scene.MapScene();
        return result;
    }
}
