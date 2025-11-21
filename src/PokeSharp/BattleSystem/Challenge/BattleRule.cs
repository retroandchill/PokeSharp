namespace PokeSharp.BattleSystem.Challenge;

public interface IBattleRule
{
    void SetRule(Battle battle);
}

public class SingleBattle : IBattleRule
{
    public void SetRule(Battle battle)
    {
        throw new NotImplementedException();
    }
}

public class DoubleBattle : IBattleRule
{
    public void SetRule(Battle battle)
    {
        throw new NotImplementedException();
    }
}
