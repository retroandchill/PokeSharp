using Injectio.Attributes;
using PokeSharp.Core.Saving;
using PokeSharp.Data;
using PokeSharp.Data.Pbs;
using PokeSharp.Trainers;

namespace PokeSharp.Saving;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class PlayerSaveData() : SaveDataValue<PlayerTrainer>("Player")
{
    protected override void Load(PlayerTrainer value)
    {
        PlayerTrainer.Instance = value;
    }

    protected override PlayerTrainer Save() => PlayerTrainer.Instance;

    protected override PlayerTrainer GetNewGameValue()
    {
        return new PlayerTrainer(TextConstants.Unnamed, TrainerType.Keys.First());
    }
}
