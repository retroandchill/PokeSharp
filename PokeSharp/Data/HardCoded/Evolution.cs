using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.HardCoded;

[GameDataEntity]
public partial record Evolution
{

    public required Name Id { get; init; }

    public required Text Name { get; init; }
    
    public object? Parameter { get; init; }
    
    public bool AnyLevelUp { get; init; }
    
    public string? LevelUpProc { get; init; }
    
    public string? UseItemProc { get; init; }
    
    public string? OnTradeProc { get; init; }
    
    public string? AfterBattleProc { get; init; }
    
    public string? EventProc { get; init; }
    
    public string? AfterEvolutionProc { get; init; }

    #region Defaults

    private const string LocalizationNamespace = "GameData.Evolution";

    public static void AddDefaultValues()
    {
        Register(
            new Evolution
            {
                Id = "Level",
                Name = Text.Localized(LocalizationNamespace, "Level", "Level"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307217918 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:72>"
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelMale",
                Name = Text.Localized(LocalizationNamespace, "LevelMale", "LevelMale"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307217828 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:80>"
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelFemale",
                Name = Text.Localized(LocalizationNamespace, "LevelFemale", "LevelFemale"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307217738 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:88>"
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelDay",
                Name = Text.Localized(LocalizationNamespace, "LevelDay", "LevelDay"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307217648 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:96>"
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelNight",
                Name = Text.Localized(LocalizationNamespace, "LevelNight", "LevelNight"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307217558 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:104>",
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelMorning",
                Name = Text.Localized(LocalizationNamespace, "LevelMorning", "LevelMorning"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307217468 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:112>"
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelAfternoon",
                Name = Text.Localized(LocalizationNamespace, "LevelAfternoon", "LevelAfternoon"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307217378 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:120>"
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelEvening",
                Name = Text.Localized(LocalizationNamespace, "LevelEvening", "LevelEvening"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307217288 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:128>",
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelNoWeather",
                Name = Text.Localized(LocalizationNamespace, "LevelNoWeather", "LevelNoWeather"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307217198 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:136>"
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelSun",
                Name = Text.Localized(LocalizationNamespace, "LevelSun", "LevelSun"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x00000253072170a8 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:144>"
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelRain",
                Name = Text.Localized(LocalizationNamespace, "LevelRain", "LevelRain"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307216fb8 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:153>"
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelSnow",
                Name = Text.Localized(LocalizationNamespace, "LevelSnow", "LevelSnow"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307216ec8 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:162>"
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelSandstorm",
                Name = Text.Localized(LocalizationNamespace, "LevelSandstorm", "LevelSandstorm"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307216dd8 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:171>"
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelCycling",
                Name = Text.Localized(LocalizationNamespace, "LevelCycling", "LevelCycling"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307216ce8 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:180>"
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelSurfing",
                Name = Text.Localized(LocalizationNamespace, "LevelSurfing", "LevelSurfing"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307216bf8 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:188>"
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelDiving",
                Name = Text.Localized(LocalizationNamespace, "LevelDiving", "LevelDiving"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307216b08 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:196>"
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelDarkness",
                Name = Text.Localized(LocalizationNamespace, "LevelDarkness", "LevelDarkness"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307216a18 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:204>"
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelDarkInParty",
                Name = Text.Localized(LocalizationNamespace, "LevelDarkInParty", "LevelDarkInParty"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307216928 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:212>"
            }
        );

        Register(
            new Evolution
            {
                Id = "AttackGreater",
                Name = Text.Localized(LocalizationNamespace, "AttackGreater", "AttackGreater"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307216838 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:220>"
            }
        );

        Register(
            new Evolution
            {
                Id = "AtkDefEqual",
                Name = Text.Localized(LocalizationNamespace, "AtkDefEqual", "AtkDefEqual"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307216748 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:228>"
            }
        );

        Register(
            new Evolution
            {
                Id = "DefenseGreater",
                Name = Text.Localized(LocalizationNamespace, "DefenseGreater", "DefenseGreater"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307216658 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:236>"
            }
        );

        Register(
            new Evolution
            {
                Id = "Silcoon",
                Name = Text.Localized(LocalizationNamespace, "Silcoon", "Silcoon"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307216568 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:244>"
            }
        );

        Register(
            new Evolution
            {
                Id = "Cascoon",
                Name = Text.Localized(LocalizationNamespace, "Cascoon", "Cascoon"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307216478 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:252>"
            }
        );

        Register(
            new Evolution
            {
                Id = "Ninjask",
                Name = Text.Localized(LocalizationNamespace, "Ninjask", "Ninjask"),
                Parameter = typeof(int),
                LevelUpProc = "#<Proc:0x0000025307216388 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:260>"
            }
        );

        Register(
            new Evolution
            {
                Id = "Shedinja",
                Name = Text.Localized(LocalizationNamespace, "Shedinja", "Shedinja"),
                Parameter = typeof(int),
                AfterEvolutionProc = "#<Proc:0x0000025307216298 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:268>",
            }
        );

        Register(
            new Evolution
            {
                Id = "Happiness",
                Name = Text.Localized(LocalizationNamespace, "Happiness", "Happiness"),
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x00000253072161d0 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:280>"
            }
        );

        Register(
            new Evolution
            {
                Id = "HappinessMale",
                Name = Text.Localized(LocalizationNamespace, "HappinessMale", "HappinessMale"),
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x0000025307216108 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:288>"
            }
        );

        Register(
            new Evolution
            {
                Id = "HappinessFemale",
                Name = Text.Localized(LocalizationNamespace, "HappinessFemale", "HappinessFemale"),
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x0000025307216040 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:296>"
            }
        );

        Register(
            new Evolution
            {
                Id = "HappinessDay",
                Name = Text.Localized(LocalizationNamespace, "HappinessDay", "HappinessDay"),
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x0000025307215f78 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:304>"
            }
        );

        Register(
            new Evolution
            {
                Id = "HappinessNight",
                Name = Text.Localized(LocalizationNamespace, "HappinessNight", "HappinessNight"),
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x0000025307215eb0 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:312>"
            }
        );

        Register(
            new Evolution
            {
                Id = "HappinessMove",
                Name = Text.Localized(LocalizationNamespace, "HappinessMove", "HappinessMove"),
                Parameter = "Move",
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x0000025307215de8 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:321>"
            }
        );

        Register(
            new Evolution
            {
                Id = "HappinessMoveType",
                Name = Text.Localized(LocalizationNamespace, "HappinessMoveType", "HappinessMoveType"),
                Parameter = "Type",
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x0000025307215d20 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:332>"
            }
        );

        Register(
            new Evolution
            {
                Id = "HappinessHoldItem",
                Name = Text.Localized(LocalizationNamespace, "HappinessHoldItem", "HappinessHoldItem"),
                Parameter = "Item",
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x0000025307215c58 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:343>",
                AfterEvolutionProc = "#<Proc:0x0000025307215c30 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:346>",
            }
        );

        Register(
            new Evolution
            {
                Id = "MaxHappiness",
                Name = Text.Localized(LocalizationNamespace, "MaxHappiness", "MaxHappiness"),
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x0000025307215b68 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:356>"
            }
        );

        Register(
            new Evolution
            {
                Id = "Beauty",
                Name = Text.Localized(LocalizationNamespace, "Beauty", "Beauty"),
                Parameter = typeof(int),
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x0000025307215a78 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:365>"
            }
        );

        Register(
            new Evolution
            {
                Id = "HoldItem",
                Name = Text.Localized(LocalizationNamespace, "HoldItem", "HoldItem"),
                Parameter = "Item",
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x00000253072159b0 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:374>",
                AfterEvolutionProc = "#<Proc:0x0000025307215988 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:377>",
            }
        );

        Register(
            new Evolution
            {
                Id = "HoldItemMale",
                Name = Text.Localized(LocalizationNamespace, "HoldItemMale", "HoldItemMale"),
                Parameter = "Item",
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x00000253072158c0 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:388>",
                AfterEvolutionProc = "#<Proc:0x0000025307215898 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:391>",
            }
        );

        Register(
            new Evolution
            {
                Id = "HoldItemFemale",
                Name = Text.Localized(LocalizationNamespace, "HoldItemFemale", "HoldItemFemale"),
                Parameter = "Item",
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x00000253072157d0 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:402>",
                AfterEvolutionProc = "#<Proc:0x00000253072157a8 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:405>",
            }
        );

        Register(
            new Evolution
            {
                Id = "DayHoldItem",
                Name = Text.Localized(LocalizationNamespace, "DayHoldItem", "DayHoldItem"),
                Parameter = "Item",
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x00000253072156e0 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:416>",
                AfterEvolutionProc = "#<Proc:0x00000253072156b8 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:419>",
            }
        );

        Register(
            new Evolution
            {
                Id = "NightHoldItem",
                Name = Text.Localized(LocalizationNamespace, "NightHoldItem", "NightHoldItem"),
                Parameter = "Item",
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x00000253072155f0 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:430>",
                AfterEvolutionProc = "#<Proc:0x00000253072155c8 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:433>",
            }
        );

        Register(
            new Evolution
            {
                Id = "HoldItemHappiness",
                Name = Text.Localized(LocalizationNamespace, "HoldItemHappiness", "HoldItemHappiness"),
                Parameter = "Item",
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x0000025307215500 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:444>",
                AfterEvolutionProc = "#<Proc:0x00000253072154d8 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:447>",
            }
        );

        Register(
            new Evolution
            {
                Id = "HasMove",
                Name = Text.Localized(LocalizationNamespace, "HasMove", "HasMove"),
                Parameter = "Move",
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x0000025307215410 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:458>"
            }
        );

        Register(
            new Evolution
            {
                Id = "HasMoveType",
                Name = Text.Localized(LocalizationNamespace, "HasMoveType", "HasMoveType"),
                Parameter = "Type",
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x0000025307215348 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:467>"
            }
        );

        Register(
            new Evolution
            {
                Id = "HasInParty",
                Name = Text.Localized(LocalizationNamespace, "HasInParty", "HasInParty"),
                Parameter = "Species",
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x0000025307215280 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:476>"
            }
        );

        Register(
            new Evolution
            {
                Id = "Location",
                Name = Text.Localized(LocalizationNamespace, "Location", "Location"),
                Parameter = typeof(int),
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x0000025307215190 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:485>"
            }
        );

        Register(
            new Evolution
            {
                Id = "LocationFlag",
                Name = Text.Localized(LocalizationNamespace, "LocationFlag", "LocationFlag"),
                Parameter = "String",
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x00000253072150a0 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:494>"
            }
        );

        Register(
            new Evolution
            {
                Id = "Region",
                Name = Text.Localized(LocalizationNamespace, "Region", "Region"),
                Parameter = typeof(int),
                AnyLevelUp = true,
                LevelUpProc = "#<Proc:0x0000025307214fb0 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:503>"
            }
        );

        Register(
            new Evolution
            {
                Id = "Item",
                Name = Text.Localized(LocalizationNamespace, "Item", "Item"),
                Parameter = "Item",
                UseItemProc = "#<Proc:0x0000025307214ee8 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:515>"
            }
        );

        Register(
            new Evolution
            {
                Id = "ItemMale",
                Name = Text.Localized(LocalizationNamespace, "ItemMale", "ItemMale"),
                Parameter = "Item",
                UseItemProc = "#<Proc:0x0000025307214e20 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:523>"
            }
        );

        Register(
            new Evolution
            {
                Id = "ItemFemale",
                Name = Text.Localized(LocalizationNamespace, "ItemFemale", "ItemFemale"),
                Parameter = "Item",
                UseItemProc = "#<Proc:0x0000025307214d58 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:531>"
            }
        );

        Register(
            new Evolution
            {
                Id = "ItemDay",
                Name = Text.Localized(LocalizationNamespace, "ItemDay", "ItemDay"),
                Parameter = "Item",
                UseItemProc = "#<Proc:0x0000025307214c90 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:539>"
            }
        );

        Register(
            new Evolution
            {
                Id = "ItemNight",
                Name = Text.Localized(LocalizationNamespace, "ItemNight", "ItemNight"),
                Parameter = "Item",
                UseItemProc = "#<Proc:0x0000025307214bc8 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:547>"
            }
        );

        Register(
            new Evolution
            {
                Id = "ItemHappiness",
                Name = Text.Localized(LocalizationNamespace, "ItemHappiness", "ItemHappiness"),
                Parameter = "Item",
                UseItemProc = "#<Proc:0x0000025307214b00 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:555>"
            }
        );

        Register(
            new Evolution
            {
                Id = "Trade",
                Name = Text.Localized(LocalizationNamespace, "Trade", "Trade"),
                OnTradeProc = "#<Proc:0x0000025307214a38 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:565>"
            }
        );

        Register(
            new Evolution
            {
                Id = "TradeMale",
                Name = Text.Localized(LocalizationNamespace, "TradeMale", "TradeMale"),
                OnTradeProc = "#<Proc:0x0000025307214970 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:572>"
            }
        );

        Register(
            new Evolution
            {
                Id = "TradeFemale",
                Name = Text.Localized(LocalizationNamespace, "TradeFemale", "TradeFemale"),
                OnTradeProc = "#<Proc:0x00000253072148a8 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:579>"
            }
        );

        Register(
            new Evolution
            {
                Id = "TradeDay",
                Name = Text.Localized(LocalizationNamespace, "TradeDay", "TradeDay"),
                OnTradeProc = "#<Proc:0x00000253072147e0 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:586>"
            }
        );

        Register(
            new Evolution
            {
                Id = "TradeNight",
                Name = Text.Localized(LocalizationNamespace, "TradeNight", "TradeNight"),
                OnTradeProc = "#<Proc:0x0000025307214718 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:593>"
            }
        );

        Register(
            new Evolution
            {
                Id = "TradeItem",
                Name = Text.Localized(LocalizationNamespace, "TradeItem", "TradeItem"),
                Parameter = "Item",
                OnTradeProc = "#<Proc:0x0000025307214650 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:601>",
                AfterEvolutionProc = "#<Proc:0x0000025307214628 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:604>",
            }
        );

        Register(
            new Evolution
            {
                Id = "TradeSpecies",
                Name = Text.Localized(LocalizationNamespace, "TradeSpecies", "TradeSpecies"),
                Parameter = "Species",
                OnTradeProc = "#<Proc:0x0000025307214560 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:614>"
            }
        );

        Register(
            new Evolution
            {
                Id = "BattleDealCriticalHit",
                Name = Text.Localized(LocalizationNamespace, "BattleDealCriticalHit", "BattleDealCriticalHit"),
                Parameter = typeof(int),
                AfterBattleProc = "#<Proc:0x0000025307214470 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:625>"
            }
        );

        Register(
            new Evolution
            {
                Id = "Event",
                Name = Text.Localized(LocalizationNamespace, "Event", "Event"),
                Parameter = typeof(int),
                EventProc = "#<Proc:0x0000025307214358 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:657>"
            }
        );

        Register(
            new Evolution
            {
                Id = "EventAfterDamageTaken",
                Name = Text.Localized(LocalizationNamespace, "EventAfterDamageTaken", "EventAfterDamageTaken"),
                Parameter = typeof(int),
                AfterBattleProc = "#<Proc:0x0000025307214268 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:665>",
                EventProc = "#<Proc:0x0000025307214240 D:/dev/pokemon-essentials-plugins/Data/Scripts/010_Data/001_Hardcoded data/007_Evolution.rb:673>"
            }
        );

    }
    #endregion
}
