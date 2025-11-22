using PokeSharp.Trainers;

namespace PokeSharp.BattleSystem.Challenge;

public record BattleType
{
    public virtual Battle CreateBattle(IBattleScreen screen, Trainer trainer1, Trainer trainer2)
    {
        throw new NotImplementedException();
    }
}

public record BattleTower : BattleType
{
    public static readonly BattleTower Default = new();

    public override Battle CreateBattle(IBattleScreen screen, Trainer trainer1, Trainer trainer2)
    {
        throw new NotImplementedException();
    }
}
