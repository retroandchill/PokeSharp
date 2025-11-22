namespace PokeSharp.BattleSystem.Challenge;

public interface IBattleRule
{
    void SetRule(Battle battle);
}

public record SingleBattle : IBattleRule
{
    public void SetRule(Battle battle)
    {
        throw new NotImplementedException();
    }
}

public record DoubleBattle : IBattleRule
{
    public void SetRule(Battle battle)
    {
        throw new NotImplementedException();
    }
}
