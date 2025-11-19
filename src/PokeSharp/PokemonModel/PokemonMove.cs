using System.Collections.Immutable;
using MessagePack;
using PokeSharp.Core;
using PokeSharp.Data.Pbs;

namespace PokeSharp.PokemonModel;

[MessagePackObject(true, AllowPrivate = true)]
public class PokemonMove
{
    public Name Id
    {
        get;
        set
        {
            field = value;
            PP = Math.Clamp(PP, 0, TotalPP);
        }
    }

    public int PP
    {
        get;
        set => field = Math.Clamp(value, field, TotalPP);
    }

    public int TotalPP
    {
        get
        {
            var maxPP = Move.Get(Id).TotalPP;
            return maxPP + maxPP * PPUps / 5;
        }
    }

    public int PPUps
    {
        get;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            field = value;
            PP = Math.Clamp(PP, 0, TotalPP);
        }
    }

    internal PokemonMove()
    {
        
    }

    public PokemonMove(Name moveId)
    {
        Id = moveId;
        PP = TotalPP;
        PPUps = 0;
    }

    public Name FunctionCode => Move.Get(Id).FunctionCode;
    public int Power => Move.Get(Id).Power;
    public Name Type => Move.Get(Id).Type;
    public DamageCategory Category => Move.Get(Id).Category;
    public bool IsPhysical => Move.Get(Id).IsPhysical;
    public bool IsSpecial => Move.Get(Id).IsSpecial;
    public bool IsStatus => Move.Get(Id).IsStatus;
    public int Accuracy => Move.Get(Id).Accuracy;
    public int EffectChance => Move.Get(Id).EffectChance;
    public Name Target => Move.Get(Id).Target;
    public int Priority => Move.Get(Id).Priority;
    public ImmutableArray<Name> Flags => Move.Get(Id).Flags;
    public Text Name => Move.Get(Id).Name;
    public Text Description => Move.Get(Id).Description;
    public bool IsHiddenMove => Move.Get(Id).IsHiddenMove;

    public PokemonMove Clone()
    {
        return new PokemonMove(Id) { PP = PP, PPUps = PPUps };
    }
}
