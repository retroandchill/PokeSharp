using Injectio.Attributes;
using PokeSharp.Core.Saving;
using PokeSharp.Core.State;
using PokeSharp.Core.Versioning;
using PokeSharp.Data;
using PokeSharp.Data.Pbs;
using PokeSharp.Items;
using PokeSharp.PokemonModel.Storage;
using PokeSharp.State;
using PokeSharp.Trainers;
using Semver;

namespace PokeSharp.Saving;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class PlayerSaveData(IGameStateAccessor<PlayerTrainer> playerTrainer)
    : SaveDataValue<PlayerTrainer>("Player")
{
    protected override void Load(PlayerTrainer value)
    {
        playerTrainer.Replace(value);
    }

    protected override PlayerTrainer Save() => playerTrainer.Current;

    protected override PlayerTrainer GetNewGameValue()
    {
        return new PlayerTrainer(TextConstants.Unnamed, TrainerType.Keys.First());
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class GameSystemSaveData : SaveDataValue<GameSystem>
{
    private readonly IGameStateAccessor<GameSystem> _gameSystem;

    public GameSystemSaveData(IGameStateAccessor<GameSystem> gameSystem)
        : base("GameSystem")
    {
        _gameSystem = gameSystem;
        LoadInBootup = true;
    }

    protected override void Load(GameSystem value)
    {
        _gameSystem.Replace(value);
    }

    protected override GameSystem Save() => _gameSystem.Current;

    protected override GameSystem GetNewGameValue()
    {
        return new GameSystem();
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class PokemonSystemSaveData : SaveDataValue<PokemonSystem>
{
    private readonly IGameStateAccessor<PokemonSystem> _pokemonSystem;

    public PokemonSystemSaveData(IGameStateAccessor<PokemonSystem> pokemonSystem)
        : base("PokemonSystem")
    {
        _pokemonSystem = pokemonSystem;
        LoadInBootup = true;
    }

    protected override void Load(PokemonSystem value)
    {
        _pokemonSystem.Replace(value);
    }

    protected override PokemonSystem Save() => _pokemonSystem.Current;

    protected override PokemonSystem GetNewGameValue()
    {
        return new PokemonSystem();
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class PokemonBagSaveData(IGameStateAccessor<PokemonBag> pokemonBag)
    : SaveDataValue<PokemonBag>("PokemonBag")
{
    protected override void Load(PokemonBag value)
    {
        pokemonBag.Replace(value);
    }

    protected override PokemonBag Save() => pokemonBag.Current;

    protected override PokemonBag GetNewGameValue()
    {
        return new PokemonBag();
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class PokemonStorageSaveData(IGameStateAccessor<IPokemonStorage> pokemonStorage)
    : SaveDataValue<IPokemonStorage>("PokemonStorage")
{
    protected override void Load(IPokemonStorage value)
    {
        pokemonStorage.Replace(value);
    }

    protected override IPokemonStorage Save() => pokemonStorage.Current;

    protected override IPokemonStorage GetNewGameValue()
    {
        return new PokemonStorage();
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class PokeSharpVersionSaveData : SaveDataValue<SemVersion>
{
    private readonly SaveGameVersions _saveGameVersions;
    private readonly VersioningService _versioningService;

    public PokeSharpVersionSaveData(SaveGameVersions saveGameVersions, VersioningService versioningService)
        : base(SaveDataService.PokeSharpVersion)
    {
        _saveGameVersions = saveGameVersions;
        _versioningService = versioningService;
        LoadInBootup = true;
    }

    protected override void Load(SemVersion value)
    {
        _saveGameVersions.PokeSharpVersion = value;
    }

    protected override SemVersion Save() => _saveGameVersions.PokeSharpVersion;

    protected override SemVersion GetNewGameValue() => _versioningService.FrameworkVersion;
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class GameVersionSaveData : SaveDataValue<SemVersion>
{
    private readonly SaveGameVersions _saveGameVersions;
    private readonly VersioningService _versioningService;

    public GameVersionSaveData(SaveGameVersions saveGameVersions, VersioningService versioningService)
        : base(SaveDataService.GameVersion)
    {
        _saveGameVersions = saveGameVersions;
        _versioningService = versioningService;
        LoadInBootup = true;
    }

    protected override void Load(SemVersion value)
    {
        _saveGameVersions.GameVersion = value;
    }

    protected override SemVersion Save() => _saveGameVersions.GameVersion;

    protected override SemVersion GetNewGameValue() => _versioningService.GameVersion;
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class GameStatsSaveData : SaveDataValue<GameStats>
{
    private readonly IGameStateAccessor<GameStats> _gameStats;

    public GameStatsSaveData(IGameStateAccessor<GameStats> gameStats)
        : base("GameStats")
    {
        _gameStats = gameStats;
        LoadInBootup = true;
        ResetOnNewGame = true;
    }

    protected override void Load(GameStats value)
    {
        _gameStats.Replace(value);
    }

    protected override GameStats Save() => _gameStats.Current;

    protected override GameStats GetNewGameValue() => new();
}
