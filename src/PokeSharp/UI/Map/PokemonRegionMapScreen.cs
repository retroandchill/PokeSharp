using PokeSharp.Data.Pbs;

namespace PokeSharp.UI.Map;

public class PokemonRegionMapScreen(IPokemonRegionMapScene scene)
{
    public async ValueTask<FlyDestination?> StartFlyScreen()
    {
        scene.StartScene(false, true);
        var result = await scene.MapScene();
        scene.EndScene();
        return result;
    }

    public async ValueTask<FlyDestination?> StartScreen()
    {
        scene.StartScene();
        var result = await scene.MapScene();
        scene.EndScene();
        return result;
    }
}
