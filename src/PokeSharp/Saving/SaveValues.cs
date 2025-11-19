using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Saving;
using PokeSharp.Core.State;
using PokeSharp.Data;
using PokeSharp.Data.Pbs;
using PokeSharp.State;
using PokeSharp.Trainers;

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
