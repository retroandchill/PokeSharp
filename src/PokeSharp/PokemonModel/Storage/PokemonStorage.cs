using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Core;
using PokeSharp.Settings;
using PokeSharp.SourceGenerator.Attributes;
using PokeSharp.Trainers;

namespace PokeSharp.PokemonModel.Storage;

[AutoServiceShortcut]
public interface IPokemonStorage
{
    bool[] UnlockedWallpapers { get; }
    ImmutableArray<Text> AllWallpapers { get; }
    IEnumerable<(int Index, Text Name)> AvailableWallpapers { get; }
    List<Pokemon> Party { get; }
    bool IsPartyFull { get; }
    int MaxBoxes { get; }
    bool IsFull { get; }
    bool IsAvailableWallpaper(int index);
    int MaxPokemon(int box);
    int? GetFirstFreePosition(int box);
    IReadOnlyList<Pokemon?> this[int box] { get; }
    Pokemon? this[int box, int index] { get; set; }
    bool Copy(int destinationBox, int? destinationIndex, int sourceBox, int sourceIndex);
    bool Move(int destinationBox, int? destinationIndex, int sourceBox, int sourceIndex);
    bool MoveCaughtToParty(Pokemon pokemon);
    bool MoveCaughtToBox(Pokemon pokemon, int box);
    int? StoreCaught(Pokemon pokemon);
    void Delete(int box, int index);
}

public class PokemonStorage : IPokemonStorage
{
    public const int PartyBox = -1;

    public PokemonBox[] Boxes { get; }

    public int CurrentBox { get; set; }

    public bool[] UnlockedWallpapers { get; set; }

    public const int BasicWallpaperQuantity = 16;

    public PokemonStorage(int? maxBoxes = null, int maxPokemon = PokemonBox.BoxSize)
    {
        var boxCount = maxBoxes ?? GameServices.GameSettings.NumStorageBoxes;
        Boxes = Enumerable
            .Range(0, boxCount)
            .Select(i => new PokemonBox($"Box {i + 1}", maxPokemon) { Background = i % BasicWallpaperQuantity })
            .ToArray();
        CurrentBox = 0;
        UnlockedWallpapers = new bool[AllWallpapers.Length];
    }

    public ImmutableArray<Text> AllWallpapers { get; } =
    [
        // Basic wallpapers
        Text.Localized("PokemonStorage.Wallpaper", "Forest", "Forest"),
        Text.Localized("PokemonStorage.Wallpaper", "City", "City"),
        Text.Localized("PokemonStorage.Wallpaper", "Desert", "Desert"),
        Text.Localized("PokemonStorage.Wallpaper", "Savanna", "Savanna"),
        Text.Localized("PokemonStorage.Wallpaper", "Crag", "Crag"),
        Text.Localized("PokemonStorage.Wallpaper", "Volcano", "Volcano"),
        Text.Localized("PokemonStorage.Wallpaper", "Snow", "Snow"),
        Text.Localized("PokemonStorage.Wallpaper", "Cave", "Cave"),
        Text.Localized("PokemonStorage.Wallpaper", "Beach", "Beach"),
        Text.Localized("PokemonStorage.Wallpaper", "Seafloor", "Seafloor"),
        Text.Localized("PokemonStorage.Wallpaper", "River", "River"),
        Text.Localized("PokemonStorage.Wallpaper", "Sky", "Sky"),
        Text.Localized("PokemonStorage.Wallpaper", "Poké Center", "Poké Center"),
        Text.Localized("PokemonStorage.Wallpaper", "Machine", "Machine"),
        Text.Localized("PokemonStorage.Wallpaper", "Checks", "Checks"),
        Text.Localized("PokemonStorage.Wallpaper", "Simple", "Simple"),
        // Special wallpapers
        Text.Localized("PokemonStorage.Wallpaper", "Space", "Space"),
        Text.Localized("PokemonStorage.Wallpaper", "Backyard", "Backyard"),
        Text.Localized("PokemonStorage.Wallpaper", "Nostalgic1", "Nostalgic 1"),
        Text.Localized("PokemonStorage.Wallpaper", "Torchic", "Torchic"),
        Text.Localized("PokemonStorage.Wallpaper", "Trio1", "Trio 1"),
        Text.Localized("PokemonStorage.Wallpaper", "PikaPika1", "PikaPika 1"),
        Text.Localized("PokemonStorage.Wallpaper", "Legend1", "Legend 1"),
        Text.Localized("PokemonStorage.Wallpaper", "Team Galactic1", "Team Galactic 1"),
        Text.Localized("PokemonStorage.Wallpaper", "Distortion", "Distortion"),
        Text.Localized("PokemonStorage.Wallpaper", "Contest", "Contest"),
        Text.Localized("PokemonStorage.Wallpaper", "Nostalgic2", "Nostalgic 2"),
        Text.Localized("PokemonStorage.Wallpaper", "Croagunk", "Croagunk"),
        Text.Localized("PokemonStorage.Wallpaper", "Trio2", "Trio 2"),
        Text.Localized("PokemonStorage.Wallpaper", "PikaPika2", "PikaPika 2"),
        Text.Localized("PokemonStorage.Wallpaper", "Legend2", "Legend 2"),
        Text.Localized("PokemonStorage.Wallpaper", "TeamGalactic2", "Team Galactic 2"),
        Text.Localized("PokemonStorage.Wallpaper", "Heart", "Heart"),
        Text.Localized("PokemonStorage.Wallpaper", "Soul", "Soul"),
        Text.Localized("PokemonStorage.Wallpaper", "BigBrother", "Big Brother"),
        Text.Localized("PokemonStorage.Wallpaper", "Pokeathlon", "Pokéathlon"),
        Text.Localized("PokemonStorage.Wallpaper", "Trio3", "Trio 3"),
        Text.Localized("PokemonStorage.Wallpaper", "SpikyPika", "Spiky Pika"),
        Text.Localized("PokemonStorage.Wallpaper", "KimonoGirl", "Kimono Girl"),
        Text.Localized("PokemonStorage.Wallpaper", "Revival", "Revival"),
    ];

    public bool IsAvailableWallpaper(int index)
    {
        return index < BasicWallpaperQuantity || UnlockedWallpapers[index];
    }

    public IEnumerable<(int Index, Text Name)> AvailableWallpapers =>
        AllWallpapers.Where((_, index) => IsAvailableWallpaper(index)).Select((text, index) => (index, text));

    public List<Pokemon> Party
    {
        get => PlayerTrainer.Instance.Party;
        set => throw new InvalidOperationException("Party cannot be set directly");
    }

    public bool IsPartyFull => PlayerTrainer.Instance.IsPartyFull;

    public int MaxBoxes => Boxes.Length;

    public int MaxPokemon(int box)
    {
        if (box >= MaxBoxes)
            return 0;

        return box < 0 ? GameServices.GameSettings.MaxPartySize : this[box].Count;
    }

    public bool IsFull => Boxes.All(b => b.IsFull);

    public int? GetFirstFreePosition(int box)
    {
        if (box == PartyBox)
        {
            var partyLength = Party.Count;
            return partyLength >= GameServices.GameSettings.MaxPartySize ? null : partyLength;
        }

        var maxPokemon = MaxPokemon(box);
        for (var i = 0; i < maxPokemon; i++)
        {
            if (i < this[box].Count)
                return i;
        }

        return null;
    }

    public IReadOnlyList<Pokemon?> this[int box] => box == PartyBox ? Party : Boxes[box];

    public Pokemon? this[int box, int index]
    {
        get => box == PartyBox ? Party[index] : Boxes[box][index];
        set
        {
            if (box == PartyBox)
            {
                ArgumentNullException.ThrowIfNull(value);
                Party[index] = value;
            }
            else
            {
                Boxes[box][index] = value;
            }
        }
    }

    public bool Copy(int destinationBox, int? destinationIndex, int sourceBox, int sourceIndex)
    {
        if (!destinationIndex.HasValue && destinationBox < MaxBoxes)
        {
            var maxPokemon = MaxPokemon(destinationBox);
            for (var i = 0; i < maxPokemon; i++)
            {
                if (this[destinationBox, i] is not null)
                    continue;
                destinationIndex = i;
                break;
            }

            if (!destinationIndex.HasValue)
                return false;
        }

        if (destinationBox == PartyBox)
        {
            if (IsPartyFull)
                return false;

            var foundPokemon = this[sourceBox, sourceIndex];
            if (foundPokemon is not null)
            {
                Party.Add(foundPokemon);
            }
        }
        else
        {
            var pokemon = this[sourceBox, sourceIndex];
            if (pokemon is null)
            {
                throw new InvalidOperationException("Trying to copy null to storage");
            }

            if (GameServices.GameSettings.HealStoredPokemon)
            {
                var oldReadyToEvolve = pokemon.HasTag(PokemonTags.ReadyToEvolve);
                pokemon.Heal();
                if (oldReadyToEvolve)
                {
                    pokemon.AddTag(PokemonTags.ReadyToEvolve);
                }
            }

            if (!destinationIndex.HasValue)
            {
                throw new InvalidOperationException($"Try to index with an invalid index.");
            }

            this[destinationBox, destinationIndex.Value] = pokemon;
        }

        return true;
    }

    public bool Move(int destinationBox, int? destinationIndex, int sourceBox, int sourceIndex)
    {
        if (!Copy(destinationBox, destinationIndex, sourceBox, sourceIndex))
            return false;

        Delete(sourceBox, sourceIndex);
        return true;
    }

    public bool MoveCaughtToParty(Pokemon pokemon)
    {
        if (IsPartyFull)
            return false;

        Party.Add(pokemon);
        return true;
    }

    public bool MoveCaughtToBox(Pokemon pokemon, int box)
    {
        var maxPokemon = MaxPokemon(box);
        for (var i = 0; i < maxPokemon; i++)
        {
            if (this[box, i] is null)
                continue;

            if (GameServices.GameSettings.HealStoredPokemon)
            {
                var oldReadyToEvolve = pokemon.HasTag(PokemonTags.ReadyToEvolve);
                pokemon.Heal();
                if (oldReadyToEvolve)
                {
                    pokemon.AddTag(PokemonTags.ReadyToEvolve);
                }
            }

            this[box, i] = pokemon;
            return true;
        }

        return false;
    }

    public int? StoreCaught(Pokemon pokemon)
    {
        if (GameServices.GameSettings.HealStoredPokemon && CurrentBox >= 0)
        {
            var oldReadyToEvolve = pokemon.HasTag(PokemonTags.ReadyToEvolve);
            pokemon.Heal();
            if (oldReadyToEvolve)
            {
                pokemon.AddTag(PokemonTags.ReadyToEvolve);
            }
        }

        var maxPokemon = MaxPokemon(CurrentBox);
        for (var i = 0; i < maxPokemon; i++)
        {
            if (this[CurrentBox, i] is not null)
                continue;

            this[CurrentBox, i] = pokemon;
            return i;
        }

        for (var i = 0; i < MaxBoxes; i++)
        {
            var maxPokemonInBox = MaxPokemon(i);
            for (var j = 0; j < maxPokemonInBox; j++)
            {
                if (this[i, j] is not null)
                    continue;

                this[i, j] = pokemon;
                return j;
            }
        }

        return null;
    }

    public void Delete(int box, int index)
    {
        if (this[box, index] is null)
            return;

        if (box == PartyBox)
        {
            Party.RemoveAt(index);
        }
        else
        {
            this[box, index] = null;
        }
    }

    public void Clear()
    {
        foreach (var box in Boxes)
        {
            box.Clear();
        }
    }
}
