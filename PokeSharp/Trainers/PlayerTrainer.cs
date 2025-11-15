using PokeSharp.Abstractions;
using PokeSharp.Core;
using PokeSharp.Core.Settings;
using PokeSharp.Core.State;
using PokeSharp.Data.Pbs;
using PokeSharp.Game;

namespace PokeSharp.Trainers;

public class PlayerTrainer(Text name, Name trainerType) : Trainer(name, trainerType)
{
    private const int BadgeNumber = 8;

    // TODO: Implement this
    public static PlayerTrainer Instance { get; } = new(Text.None, Abstractions.Name.None);

    public int CharacterId
    {
        get;
        set
        {
            if (field == value)
                return;

            field = value;
            GameServices.GamePlayer.RefreshCharset();
        }
    } = 0;

    public int Outfit
    {
        get;
        set
        {
            if (field == value)
                return;

            field = value;
            GameServices.GamePlayer.RefreshCharset();
        }
    } = 0;

    public List<bool> Badges { get; set; } = Enumerable.Repeat(false, BadgeNumber).ToList();

    public int BadgeCount => Badges.Count(b => b);

    public int Money
    {
        get;
        set => field = Math.Clamp(value, 0, GameServices.GameSettings.MaxMoney);
    } = Metadata.Instance.StartMoney;

    public int Coins
    {
        get;
        set => field = Math.Clamp(value, 0, GameServices.GameSettings.MaxCoins);
    } = 0;

    public int BattlePoints
    {
        get;
        set => field = Math.Clamp(value, 0, GameServices.GameSettings.MaxBattlePoints);
    } = 0;

    public int Soot
    {
        get;
        set => field = Math.Clamp(value, 0, GameServices.GameSettings.MaxSoot);
    } = 0;

    public Pokedex Pokedex { get; } = new();

    public bool HasPokedex { get; set; }

    public bool HasPokegear { get; set; }

    public bool HasRunningShoes { get; set; }

    public bool HasBoxLink { get; set; }

    public bool SeenStorageCreator { get; set; }

    public bool HasExpAll { get; set; }

    public bool MysteryGiftUnlocked { get; set; }

    public List<object> MysteryGifts { get; set; } = [];
}
