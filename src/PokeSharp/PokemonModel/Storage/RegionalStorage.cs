using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Core;
using PokeSharp.State;
using PokeSharp.Utilities;

namespace PokeSharp.PokemonModel.Storage;

public class RegionalStorage : IPokemonStorage
{
    private readonly Dictionary<int, PokemonStorage> _storages = new();
    private int? _lastMap;
    private int? _regionMap;

    public PokemonStorage CurrentStorage
    {
        get
        {
            var gameMap = GameServices.GameMap;
            if (_lastMap != gameMap.MapId)
            {
                _regionMap = GameplayUtils.CurrentRegion;
                _lastMap = gameMap.MapId;
            }

            if (!_regionMap.HasValue)
            {
                throw new InvalidOperationException(
                    "The current map has no region set. Please set the MapPosition metadata setting for this map."
                );
            }

            if (_storages.TryGetValue(_regionMap.Value, out var storage))
                return storage;

            storage = new PokemonStorage();
            _storages.Add(_regionMap.Value, storage);
            return storage;
        }
    }

    public bool[] UnlockedWallpapers => CurrentStorage.UnlockedWallpapers;

    public ImmutableArray<Text> AllWallpapers => CurrentStorage.AllWallpapers;

    public IEnumerable<(int Index, Text Name)> AvailableWallpapers => CurrentStorage.AvailableWallpapers;

    public bool IsAvailableWallpaper(int index)
    {
        throw new NotImplementedException();
    }

    public PokemonBox[] Boxes => CurrentStorage.Boxes;

    public List<Pokemon> Party => CurrentStorage.Party;

    public bool IsPartyFull => CurrentStorage.IsPartyFull;

    public int MaxBoxes => CurrentStorage.MaxBoxes;

    public int MaxPokemon(int box) => CurrentStorage.MaxPokemon(box);

    public int? GetFirstFreePosition(int box)
    {
        throw new NotImplementedException();
    }

    public bool IsFull => CurrentStorage.IsFull;

    public int CurrentBox
    {
        get => CurrentStorage.CurrentBox;
        set => CurrentStorage.CurrentBox = value;
    }

    public IReadOnlyList<Pokemon?> this[int box] => CurrentStorage[box];

    public Pokemon? this[int box, int index]
    {
        get => CurrentStorage[box, index];
        set => CurrentStorage[box, index] = value;
    }

    public bool Copy(int destinationBox, int? destinationIndex, int sourceBox, int sourceIndex)
    {
        return CurrentStorage.Copy(destinationBox, destinationIndex, sourceBox, sourceIndex);
    }

    public bool Move(int destinationBox, int? destinationIndex, int sourceBox, int sourceIndex)
    {
        return CurrentStorage.Move(destinationBox, destinationIndex, sourceBox, sourceIndex);
    }

    public bool MoveCaughtToParty(Pokemon pokemon)
    {
        return CurrentStorage.MoveCaughtToParty(pokemon);
    }

    public bool MoveCaughtToBox(Pokemon pokemon, int box)
    {
        return CurrentStorage.MoveCaughtToBox(pokemon, box);
    }

    public int? StoreCaught(Pokemon pokemon)
    {
        return CurrentStorage.StoreCaught(pokemon);
    }

    public void Delete(int box, int index)
    {
        CurrentStorage.Delete(box, index);
    }
}
