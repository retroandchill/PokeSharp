namespace PokeSharp.PokemonModel.Storage;

public static class PokemonStorageExtensions
{
    extension(IPokemonStorage storage)
    {
        public void UnlockWallpaper(int index)
        {
            storage.UnlockedWallpapers[index] = true;
        }

        public void LockWallpaper(int index)
        {
            storage.UnlockedWallpapers[index] = false;
        }

        public IEnumerable<Pokemon> AllPokemon
        {
            get
            {
                for (var i = PokemonStorage.PartyBox; i < storage.MaxBoxes; i++)
                {
                    var maxPokemon = storage.MaxPokemon(i);
                    for (var j = 0; j < maxPokemon; j++)
                    {
                        var pokemon = storage[i, j];
                        if (pokemon is not null)
                            yield return pokemon;
                    }
                }
            }
        }

        public IEnumerable<Pokemon> AllNonEggPokemon => storage.AllPokemon.Where(p => !p.IsEgg);
    }
}
