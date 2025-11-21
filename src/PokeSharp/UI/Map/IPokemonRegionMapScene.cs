using PokeSharp.Core;
using PokeSharp.Data.Pbs;

namespace PokeSharp.UI.Map;

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
