namespace PokeSharp.UI.Map;

public interface IPokemonRegionMapSceneFactory
{
    IPokemonRegionMapScene CreateScene(int? region = null, bool wallMap = true);
}
