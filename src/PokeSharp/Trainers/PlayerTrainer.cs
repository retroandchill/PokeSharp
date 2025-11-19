using PokeSharp.Core;
using PokeSharp.Settings;
using PokeSharp.State;

namespace PokeSharp.Trainers;

public class PlayerTrainer(Text name, Name trainerType) : Trainer(name, trainerType)
{
    private const int BadgeNumber = 8;

    public static PlayerTrainer Instance { get; internal set; } = new(Text.None, Core.Name.None);

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
    } = GameServices.GameSettings.StartMoney;

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

    // TODO: We need to figure out the actual type for this
    public List<object> MysteryGifts { get; set; } = [];

    public bool HasSeen(Name species) => Pokedex.HasSeen(species);

    public bool Owns(Name species) => Pokedex.Owns(species);
}
