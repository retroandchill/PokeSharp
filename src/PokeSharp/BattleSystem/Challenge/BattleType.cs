using PokeSharp.Trainers;

namespace PokeSharp.BattleSystem.Challenge;

public class BattleType
{
    public virtual Battle CreateBattle(IBattleScreen screen, Trainer trainer1, Trainer trainer2)
    {
        throw new NotImplementedException();
    }
}

public class BattleTower : BattleType
{
    public override Battle CreateBattle(IBattleScreen screen, Trainer trainer1, Trainer trainer2)
    {
        throw new NotImplementedException();
    }
}
