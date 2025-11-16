using System.Collections.Immutable;
using Injectio.Attributes;
using PokeSharp.Abstractions;
using PokeSharp.Core;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;
using PokeSharp.Services.Evolution;
using PokeSharp.Services.Happiness;
using PokeSharp.Services.Healing;
using PokeSharp.Services.Moves;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.PokemonModel.Components;

public enum HeartGaugeChangeMethod : byte
{
    Battle,
    Call,
    Walking,
    Scent,
}

public readonly record struct HeartGaugeChangeAmounts(int Battle, int Call, int Walking, int Scent);

public class ShadowPokemonComponent(Pokemon pokemon) : IPokemonComponent<ShadowPokemonComponent>
{
    private Pokemon _pokemon = pokemon;

    public static Name ComponentId => ShadowPokemonComponentExtensions.ComponentId;
    public Name Id => ShadowPokemonComponentExtensions.ComponentId;

    public bool IsShadow { get; set; }

    public int HeartGauge { get; set; }

    public bool HyperMode
    {
        get => HeartGauge > 0 && _pokemon.HP > 0 && field;
        set;
    }

    public int SavedExp { get; set; }

    public Dictionary<Name, int> SavedEVs { get; set; } = new();

    public List<Name> ShadowMoves { get; set; } = [];

    public int HeartGaugeStepCount { get; set; }

    public int HeartStage
    {
        get
        {
            if (!IsShadow)
                return 0;

            var maxSize = MaxGaugeSize;
            var stageSize = maxSize / 5.0f;
            return (int)Math.Ceiling(Math.Min(maxSize, HeartGauge) / stageSize);
        }
    }

    public ShadowPokemon? ShadowData =>
        ShadowPokemon.TryGetSpeciesForm(_pokemon.Species, _pokemon.FormSimple, out var result) ? result : null;

    public int MaxGaugeSize => ShadowData?.GaugeSize ?? ShadowPokemon.MaxGaugeSize;

    public void AdjustHeart(int amount)
    {
        if (!IsShadow)
            return;

        HeartGauge = Math.Clamp(HeartGauge + amount, 0, MaxGaugeSize);
    }

    public void ChangeHeartGauge(HeartGaugeChangeMethod method, float multiplier = 1)
    {
        if (!IsShadow)
            return;

        var amounts = GameServices.ShadowPokemonHandler.GetAmounts(_pokemon.NatureId ?? Name.None);
        var amount = method switch
        {
            HeartGaugeChangeMethod.Battle => amounts.Battle,
            HeartGaugeChangeMethod.Call => amounts.Call,
            HeartGaugeChangeMethod.Walking => amounts.Walking,
            HeartGaugeChangeMethod.Scent => (int)Math.Round(amounts.Scent * multiplier),
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null),
        };

        AdjustHeart(-amount);
    }

    private static readonly Name ShadowRush = "SHADOWRUSH";

    public void MakeShadow()
    {
        IsShadow = true;
        HyperMode = false;
        SavedExp = 0;
        SavedEVs.Clear();
        foreach (var stat in Stat.AllMain)
        {
            SavedEVs.Add(stat.Id, 0);
        }
        HeartGauge = MaxGaugeSize;
        ShadowMoves.Clear();

        var data = ShadowData;
        if (data is not null)
        {
            foreach (var moveId in data.Moves)
            {
                ShadowMoves.Add(moveId);
                if (ShadowMoves.Count >= Pokemon.MaxMoves)
                    break;
            }
        }

        if (ShadowMoves.Count == 0 && Move.Exists(ShadowRush))
        {
            ShadowMoves.Add(ShadowRush);
        }

        if (ShadowMoves.Count == 0)
            return;

        for (var i = ShadowMoves.Count; i < Pokemon.MaxMoves; i++)
        {
            ShadowMoves.Add(Name.None);
        }

        foreach (var move in _pokemon.Moves)
        {
            ShadowMoves.Add(move.Id);
        }

        UpdateShadowMoves();
    }

    public void UpdateShadowMoves()
    {
        if (ShadowMoves.Count == 0)
            return;

        if (!IsShadow)
        {
            if (ShadowMoves.Count > Pokemon.MaxMoves)
            {
                var restoredMoves = ShadowMoves.Skip(Pokemon.MaxMoves).ToArray();
                ReplaceMoves(restoredMoves);
            }

            ShadowMoves.Clear();
            return;
        }

        var newMoves = ShadowMoves.Take(Pokemon.MaxMoves).Where(m => m.IsValid).ToList();
        var numShadowMoves = newMoves.Count;

        var numOriginalMoves = HeartStage switch
        {
            <= 1 => 3,
            2 => 2,
            < 5 => 1,
            _ => 0,
        };
        if (numOriginalMoves > 0)
        {
            var relearnedCount = 0;
            foreach (var move in ShadowMoves.Skip(Pokemon.MaxMoves + numShadowMoves).Where(m => m.IsValid))
            {
                newMoves.Add(move);
                relearnedCount++;
                if (relearnedCount >= numOriginalMoves)
                    break;
            }
        }

        ReplaceMoves(newMoves);
    }

    private void ReplaceMoves(IReadOnlyCollection<Name> newMoves)
    {
        _pokemon.Moves = _pokemon.Moves.Where(x => newMoves.Contains(x.Id)).ToList();

        foreach (var move in newMoves)
        {
            if (_pokemon.MoveCount >= Pokemon.MaxMoves)
                break;
            _pokemon.LearnMove(move);
        }
    }

    public bool IsPurifiable => IsShadow && HeartGauge == 0 && GameServices.ShadowPokemonHandler.IsPurifiable(_pokemon);

    public IPokemonComponent Clone(Pokemon newPokemon)
    {
        var result = (ShadowPokemonComponent)MemberwiseClone();
        result._pokemon = newPokemon;
        result.SavedEVs = new Dictionary<Name, int>(SavedEVs);
        result.ShadowMoves = new List<Name>(ShadowMoves);
        return result;
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class ShadowPokemonComponentFactory : IPokemonComponentFactory
{
    public int Priority => 20;

    public bool CanCreate(Pokemon pokemon) => true;

    public IPokemonComponent Create(Pokemon pokemon) => new ShadowPokemonComponent(pokemon);
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
[AutoServiceShortcut]
public class ShadowPokemonHandler(
    IEnumerable<IHeartGaugeChangeAmountsProvider> providers,
    IEnumerable<IPurifiableEvaluator> purifiableEvaluators
) : IPokemonFaintHandler, ICanEvolveEvaluator, IRelearnMoveChecker, IHappinessChangeBlocker
{
    private readonly Dictionary<Name, HeartGaugeChangeAmounts> _amounts = providers
        .SelectMany(x => x.GetAmounts())
        .ToDictionary();

    private readonly ImmutableArray<IPurifiableEvaluator> _purifiableEvaluators =
    [
        .. purifiableEvaluators.OrderBy(e => e.Priority),
    ];

    int IPokemonFaintHandler.Priority => 30;

    int ICanEvolveEvaluator.Priority => 10;
    int IRelearnMoveChecker.Priority => 20;
    int IHappinessChangeBlocker.Priority => 10;

    public HeartGaugeChangeAmounts GetAmounts(Name nature)
    {
        return _amounts.TryGetValue(nature, out var result) ? result : new HeartGaugeChangeAmounts(100, 100, 100, 100);
    }

    public bool IsPurifiable(Pokemon pokemon)
    {
        return _purifiableEvaluators.All(e => e.IsPurifiable(pokemon));
    }

    public bool CanEvolve(Pokemon pokemon)
    {
        return !pokemon.IsShadow;
    }

    public void OnFaint(Pokemon pokemon)
    {
        pokemon.HyperMode = false;
    }

    public bool CanRelearnMoves(Pokemon pokemon)
    {
        return !pokemon.IsShadow;
    }

    public bool ShouldBlockHappinessChange(Pokemon pokemon)
    {
        return pokemon.ShadowPokemonComponent is { IsShadow: true, HeartStage: >= 4 };
    }
}

public interface IHeartGaugeChangeAmountsProvider
{
    IEnumerable<KeyValuePair<Name, HeartGaugeChangeAmounts>> GetAmounts();
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class DefaultHeartGaugeChangeAmountsProvider : IHeartGaugeChangeAmountsProvider
{
    private readonly Dictionary<Name, HeartGaugeChangeAmounts> _amounts = new()
    {
        [Nature.Hardy.Id] = new HeartGaugeChangeAmounts(110, 300, 100, 90),
        [Nature.Lonely.Id] = new HeartGaugeChangeAmounts(70, 330, 100, 130),
        [Nature.Brave.Id] = new HeartGaugeChangeAmounts(130, 270, 90, 80),
        [Nature.Adamant.Id] = new HeartGaugeChangeAmounts(110, 270, 110, 80),
        [Nature.Naughty.Id] = new HeartGaugeChangeAmounts(120, 270, 110, 70),
        [Nature.Bold.Id] = new HeartGaugeChangeAmounts(110, 270, 90, 100),
        [Nature.Docile.Id] = new HeartGaugeChangeAmounts(100, 360, 80, 120),
        [Nature.Relaxed.Id] = new HeartGaugeChangeAmounts(90, 270, 110, 100),
        [Nature.Impish.Id] = new HeartGaugeChangeAmounts(120, 300, 100, 80),
        [Nature.Lax.Id] = new HeartGaugeChangeAmounts(100, 270, 90, 110),
        [Nature.Timid.Id] = new HeartGaugeChangeAmounts(70, 330, 110, 120),
        [Nature.Hasty.Id] = new HeartGaugeChangeAmounts(130, 300, 70, 100),
        [Nature.Serious.Id] = new HeartGaugeChangeAmounts(100, 330, 110, 90),
        [Nature.Jolly.Id] = new HeartGaugeChangeAmounts(120, 300, 90, 90),
        [Nature.Naive.Id] = new HeartGaugeChangeAmounts(100, 300, 120, 80),
        [Nature.Modest.Id] = new HeartGaugeChangeAmounts(70, 300, 120, 110),
        [Nature.Mild.Id] = new HeartGaugeChangeAmounts(80, 270, 100, 120),
        [Nature.Quiet.Id] = new HeartGaugeChangeAmounts(100, 300, 100, 100),
        [Nature.Bashful.Id] = new HeartGaugeChangeAmounts(80, 300, 90, 130),
        [Nature.Rash.Id] = new HeartGaugeChangeAmounts(90, 300, 90, 120),
        [Nature.Calm.Id] = new HeartGaugeChangeAmounts(80, 300, 110, 110),
        [Nature.Gentle.Id] = new HeartGaugeChangeAmounts(70, 300, 130, 100),
        [Nature.Sassy.Id] = new HeartGaugeChangeAmounts(130, 240, 100, 70),
        [Nature.Careful.Id] = new HeartGaugeChangeAmounts(90, 300, 100, 110),
        [Nature.Quirky.Id] = new HeartGaugeChangeAmounts(130, 270, 80, 90),
    };

    public IEnumerable<KeyValuePair<Name, HeartGaugeChangeAmounts>> GetAmounts() => _amounts;
}

public interface IPurifiableEvaluator
{
    int Priority { get; }

    bool IsPurifiable(Pokemon pokemon);
}

public sealed class ShadowLugiaPurifiableEvaluator : IPurifiableEvaluator
{
    public int Priority => 10;
    private static readonly Name Lugia = "LUGIA";

    public bool IsPurifiable(Pokemon pokemon)
    {
        return pokemon.IsSpecies(Lugia);
    }
}

public static class ShadowPokemonComponentExtensions
{
    internal static readonly Name ComponentId = "ShadowPokemon";
    private static readonly Dictionary<Name, int> DefaultSavedEVs = new();
    private static readonly List<Name> DefaultSavedMoves = [];

    extension(Pokemon pokemon)
    {
        public ShadowPokemonComponent? ShadowPokemonComponent => pokemon.GetComponent<ShadowPokemonComponent>();

        public bool IsShadow
        {
            get => pokemon.ShadowPokemonComponent?.IsShadow ?? false;
            set => pokemon.ShadowPokemonComponent?.IsShadow = value;
        }

        public int HeartGauge
        {
            get => pokemon.ShadowPokemonComponent?.HeartGauge ?? 0;
            set => pokemon.ShadowPokemonComponent?.HeartGauge = value;
        }

        public bool HyperMode
        {
            get => pokemon.ShadowPokemonComponent?.HyperMode ?? false;
            set => pokemon.ShadowPokemonComponent?.HyperMode = value;
        }

        public int SavedExp
        {
            get => pokemon.ShadowPokemonComponent?.SavedExp ?? 0;
            set => pokemon.ShadowPokemonComponent?.SavedExp = value;
        }

        public Dictionary<Name, int> SavedEVs
        {
            get => pokemon.ShadowPokemonComponent?.SavedEVs ?? DefaultSavedEVs;
            set => pokemon.ShadowPokemonComponent?.SavedEVs = value;
        }

        public List<Name> ShadowMoves
        {
            get => pokemon.ShadowPokemonComponent?.ShadowMoves ?? DefaultSavedMoves;
            set => pokemon.ShadowPokemonComponent?.ShadowMoves = value;
        }

        public int HeartGaugeStepCount
        {
            get => pokemon.ShadowPokemonComponent?.HeartGaugeStepCount ?? 0;
            set => pokemon.ShadowPokemonComponent?.HeartGaugeStepCount = value;
        }
    }
}
