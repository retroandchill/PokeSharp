using PokeSharp.Core;
using PokeSharp.Data.Pbs;

namespace PokeSharp.UI;

public interface IPokemonRegionMapScene
{
    void StartScene(bool asEditor = false, bool flyMap = false);

    void EndScene();

    float PointXToScreenX(int x);

    float PointYToScreenY(int y);

    bool LocationShown(Point point);

    void SaveMapData();

    ValueTask SaveMapDataAsync(CancellationToken cancellationToken = default);

    Text GetMapLocation(int x, int y);

    void ChangeMapLocation(int x, int y);

    Text GetMapDetails(int x, int y);

    FlyDestination? GetHealingSpot(int x, int y);

    void RefreshFlyScreen();

    ValueTask<FlyDestination?> MapScene();
}

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

public interface IPokemonRegionMapSceneFactory
{
    IPokemonRegionMapScene CreateScene(int? region = null, bool wallMap = true);
}
