using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using MessagePack;
using PokeSharp.Core;
using PokeSharp.Core.Collections;
using PokeSharp.Core.Engine;
using PokeSharp.Core.Strings;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;
using PokeSharp.Items;
using PokeSharp.PokemonModel.Forms;
using PokeSharp.Serialization.MessagePack;
using PokeSharp.Services.Evolution;
using PokeSharp.Services.Happiness;
using PokeSharp.Services.Healing;
using PokeSharp.Services.Moves;
using PokeSharp.Services.Screens;
using PokeSharp.Settings;
using PokeSharp.State;
using PokeSharp.Trainers;
using ZLinq;

namespace PokeSharp.PokemonModel;

public enum ObtainMethod : byte
{
    Met = 0,
    Egg = 1,
    Traded = 2,
    FatefulEncounter = 4,
}

public readonly record struct WildHoldItems(
    ImmutableArray<Name> Common,
    ImmutableArray<Name> Uncommon,
    ImmutableArray<Name> Rare
);

public enum PokerusStage : byte
{
    NotInfected,
    Infected,
    Cured,
}

[MessagePackObject(true, AllowPrivate = true)]
public partial class Pokemon
{
    public const int IVStatLimit = 31;
    public const int EVLimit = 510;
    public const int EVStatLimit = 252;
    public const int NameSizeLimit = 12;
    public const int MaxHappiness = 255;
    public const int MaxMoves = 4;

    #region Species and form
    private Name _species;
    public Name Species
    {
        get => _species;
        set
        {
            var newSpeciesData = Data.Pbs.Species.Get(value);
            SetSpecies(newSpeciesData);
        }
    }

    public void SetSpecies(Species newSpeciesData)
    {
        if (_species == newSpeciesData.SpeciesId)
            return;

        _species = newSpeciesData.SpeciesId;
        var defaultForm = newSpeciesData.DefaultForm;
        if (defaultForm.HasValue)
        {
            _form = defaultForm.Value;
        }
        else if (newSpeciesData.Form > 0)
        {
            _form = newSpeciesData.Form;
        }

        ForcedForm = null;
        if (SingleGendered)
            _gender = null;
        _level = null;
        _ability = null;
        CalcStats();
    }

    public bool IsSpecies(Name species) => Species == species;

    [IgnoreMember]
    public Species SpeciesData => Data.Pbs.Species.Get(Species, FormSimple);

    private int _form;

    [IgnoreMember]
    public int Form
    {
        get
        {
            if (ForcedForm.HasValue)
                return ForcedForm.Value;
            if (GameGlobal.GameTemp.InBattle || GameGlobal.GameTemp.InStorage)
                return _form;

            var calculatedForm = MultipleForms.GetForm(this);
            if (calculatedForm.HasValue && calculatedForm.Value != _form)
            {
                _form = calculatedForm.Value;
            }

            return _form;
        }
        set
        {
            var oldForm = _form;
            _form = value;
            _ability = null;
            MultipleForms.OnFormSet(this, value, oldForm);
            CalcStats();
            GameGlobal.PlayerTrainer.Pokedex.Register(this);
        }
    }

    [IgnoreMember]
    public int FormSimple
    {
        get => ForcedForm ?? _form;
        set
        {
            _form = value;
            CalcStats();
        }
    }

    public int? ForcedForm { get; set; }

    public DateTimeOffset? TimeFormSet { get; set; }

    #endregion

    #region Level
    private int? _level;

    [IgnoreMember]
    public int Level
    {
        get
        {
            _level ??= GrowthRate.GetLevelFromExp(_exp);
            return _level.Value;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, GrowthRate.MaxLevel);
            _exp = GrowthRate.GetMinimumExpForLevel(value);
            _level = value;
        }
    }

    private int _exp;

    [IgnoreMember]
    public int Exp
    {
        get => _exp;
        set
        {
            _exp = value;
            _level = null;
        }
    }

    [IgnoreMember]
    public bool IsEgg => StepsToHatch > 0;

    public int StepsToHatch { get; set; }

    [IgnoreMember]
    public GrowthRate GrowthRate => GrowthRate.Get(SpeciesData.GrowthRate);

    [IgnoreMember]
    public int BaseExp => SpeciesData.BaseExp;

    [IgnoreMember]
    public float ExpFraction
    {
        get
        {
            var level = Level;
            if (Level >= GrowthRate.MaxLevel)
                return 0.0f;

            var growthRate = GrowthRate;
            var startExp = growthRate.GetMinimumExpForLevel(level);
            var endExp = growthRate.GetMinimumExpForLevel(level + 1);
            return (float)(Exp - startExp) / (endExp - startExp);
        }
    }

    #endregion

    #region Status

    public int HP
    {
        get;
        set
        {
            field = Math.Clamp(value, 0, MaxHP);
            if (field != 0)
                return;
            HealStatus();
            GameGlobal.PokemonStatusService.OnFainted(this);
        }
    }

    public Name Status
    {
        get;
        set
        {
            if (value.IsValid && !Data.Core.Status.Exists(value))
            {
                throw new InvalidOperationException($"Attempted to set {value} as Pokémon status");
            }

            field = value;
        }
    }

    public int StatusCount { get; private set; }

    [IgnoreMember]
    public bool IsAble => !IsEgg && HP > 0;

    [IgnoreMember]
    public bool IsFainted => !IsEgg && HP <= 0;

    public void HealHP()
    {
        if (IsEgg)
            return;
        HP = MaxHP;
    }

    public void HealStatus()
    {
        if (IsEgg)
            return;

        Status = Core.Strings.Name.None;
        StatusCount = 0;
    }

    public void HealPP()
    {
        if (IsEgg)
            return;

        foreach (var move in Moves)
        {
            move.PP = move.TotalPP;
        }
    }

    public void HealPP(int moveIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(moveIndex);
        if (IsEgg)
            return;

        Moves[moveIndex].PP = Moves[moveIndex].TotalPP;
    }

    public void Heal()
    {
        if (IsEgg)
            return;

        HealHP();
        HealStatus();
        HealPP();

        GameGlobal.PokemonStatusService.OnFullyHealed(this);
    }

    #endregion

    #region Types

    [IgnoreMember]
    public ImmutableArray<Name> Types => SpeciesData.Types;

    public bool HasType(Name type) => Types.Contains(type);

    #endregion

    #region Gender

    private PokemonGender? _gender;

    [IgnoreMember]
    public PokemonGender Gender
    {
        get
        {
            if (_gender.HasValue)
                return _gender.Value;

            var genderRatio = GenderRatio.Get(SpeciesData.GenderRatio);
            _gender = genderRatio.Ratio.Match(
                femaleChance => (PersonalityValue & 0xFF) < femaleChance ? PokemonGender.Female : PokemonGender.Male,
                gender => gender
            );

            return _gender.Value;
        }
        set
        {
            if (SingleGendered || value == PokemonGender.Genderless)
                return;

            _gender = value;
        }
    }

    public void MakeMale() => Gender = PokemonGender.Male;

    public void MakeFemale() => Gender = PokemonGender.Female;

    public void ClearGender() => _gender = null;

    [IgnoreMember]
    public bool IsMale => Gender == PokemonGender.Male;

    [IgnoreMember]
    public bool IsFemale => Gender == PokemonGender.Female;

    [IgnoreMember]
    public bool SingleGendered => GenderRatio.Get(SpeciesData.GenderRatio).IsSingleGender;

    #endregion

    #region Shininess

    private bool? _shiny;

    [IgnoreMember]
    public bool Shiny
    {
        get
        {
            if (_shiny.HasValue)
                return _shiny.Value;

            _shiny = ShinyValue < GameGlobal.GameSettings.ShinyPokemonChance;

            return _shiny.Value;
        }
        set => _shiny = value;
    }

    private bool? _superShiny;

    [IgnoreMember]
    public bool SuperShiny
    {
        get
        {
            if (_superShiny.HasValue)
                return _superShiny.Value;

            _superShiny = ShinyValue == 0;
            return _superShiny.Value;
        }
        set
        {
            _superShiny = value;
            if (_superShiny.Value)
                Shiny = true;
        }
    }

    public void ResetShiny()
    {
        _shiny = null;
        _superShiny = null;
    }

    [IgnoreMember]
    private uint ShinyValue
    {
        get
        {
            var a = PersonalityValue ^ 0xFFFF;
            var b = a & 0xFFFF;
            var c = (a >> 16) & 0xFFFF;
            return b ^ c;
        }
    }

    #endregion

    #region Ability

    private int? _abilityIndex;

    [IgnoreMember]
    public int AbilityIndex
    {
        get
        {
            if (_abilityIndex.HasValue)
                return _abilityIndex.Value;

            _abilityIndex = (int)(PersonalityValue % 2);
            return _abilityIndex.Value;
        }
        set
        {
            _abilityIndex = value;
            _ability = null;
        }
    }

    public void ResetAbilityIndex()
    {
        _abilityIndex = null;
        _ability = null;
    }

    [IgnoreMember]
    public Ability? Ability => Ability.TryGet(AbilityId, out var ability) ? ability : null;

    private Name? _ability;

    [IgnoreMember]
    public Name AbilityId
    {
        get
        {
            if (_ability.HasValue)
                return _ability.Value;

            var spData = SpeciesData;
            var abilityIndex = AbilityIndex;

            if (abilityIndex >= 2)
            {
                var hiddenAbilityIndex = abilityIndex - 2;
                if (spData.HiddenAbilities.Length > hiddenAbilityIndex)
                {
                    _ability = spData.HiddenAbilities[hiddenAbilityIndex];
                    abilityIndex = (int)(PersonalityValue % 2);
                }
                else
                {
                    _ability = null;
                }
            }

            if (!_ability.HasValue)
            {
                if (abilityIndex < spData.Abilities.Length)
                {
                    _ability = spData.Abilities[abilityIndex];
                }
                else if (spData.Abilities.Length > 0)
                {
                    _ability = spData.Abilities[0];
                }
                else
                {
                    _ability = Core.Strings.Name.None;
                }
            }

            return _ability.Value;
        }
    }

    public void SetAbility(Name? ability)
    {
        if (ability.HasValue && !Ability.Exists(ability.Value))
            return;
        _ability = ability;
    }

    [IgnoreMember]
    public bool HasAbility => Ability is not null;

    public bool HasSpecificAbility(Name checkAbility)
    {
        return AbilityId == checkAbility;
    }

    [IgnoreMember]
    public bool HasHiddenAbility => AbilityIndex >= 2;

    [IgnoreMember]
    public IEnumerable<(Name Id, int Index)> AbilityList
    {
        get
        {
            var spData = SpeciesData;
            foreach (var (i, ability) in spData.Abilities.Index())
            {
                yield return (ability, i);
            }

            foreach (var (i, ability) in spData.HiddenAbilities.Index())
            {
                yield return (ability, i + 2);
            }
        }
    }

    #endregion

    #region Nature

    public Name? NatureId
    {
        get;
        set
        {
            if (value.HasValue && !Nature.Exists(value.Value))
                return;

            field = value;

            if (NatureForStats is null)
            {
                CalcStats();
            }
        }
    }

    [IgnoreMember]
    public Nature? Nature
    {
        get
        {
            // ReSharper disable once InvertIf
            if (!NatureId.HasValue)
            {
                var index = PersonalityValue % Nature.Count;
                NatureId = Nature.Keys.ElementAt((int)index);
            }

            return Nature.TryGet(NatureId.Value, out var nature) ? nature : null;
        }
        set => NatureId = value?.Id;
    }

    public Name? NatureForStatsId
    {
        get;
        set
        {
            if (value.HasValue && !Nature.Exists(value.Value))
                return;

            field = value;
            CalcStats();
        }
    }

    [IgnoreMember]
    public Nature? NatureForStats
    {
        get
        {
            if (NatureForStatsId.HasValue)
            {
                return Nature.TryGet(NatureForStatsId.Value, out var nature) ? nature : null;
            }

            return Nature;
        }
        set => NatureForStatsId = value?.Id;
    }

    [IgnoreMember]
    public bool HasNature => NatureId.HasValue;

    public bool HasSpecificNature(Name checkNature)
    {
        return NatureId == checkNature;
    }

    #endregion

    #region Items

    public Name? ItemId
    {
        get;
        set
        {
            if (value.HasValue && !Item.Exists(value.Value))
                return;

            field = value;
        }
    }

    [IgnoreMember]
    public Item? Item
    {
        get => Item.TryGet(ItemId ?? Core.Strings.Name.None, out var item) ? item : null;
        set => ItemId = value?.Id;
    }

    [IgnoreMember]
    [MemberNotNullWhen(true, nameof(ItemId))]
    [MemberNotNullWhen(true, nameof(Item))]
    public bool HasItem => ItemId.HasValue;

    public bool HasSpecificItem(Name checkItem)
    {
        return ItemId == checkItem;
    }

    [IgnoreMember]
    public WildHoldItems WildHoldItems
    {
        get
        {
            var spData = SpeciesData;
            return new WildHoldItems(spData.WildItemCommon, spData.WildItemUncommon, spData.WildItemRare);
        }
    }

    public Mail? Mail
    {
        get
        {
            if (field is not null && (field.Item.IsNone || HasSpecificItem(field.Item)))
            {
                field = null;
            }

            return field;
        }
        set;
    }

    #endregion

    #region Moves
    public List<PokemonMove> Moves { get; set; } = [];

    [IgnoreMember]
    public int MoveCount => Moves.Count;

    public bool HasMove(Name moveId)
    {
        return Moves.Any(m => m.Id == moveId);
    }

    [IgnoreMember]
    public ImmutableArray<LevelUpMove> MoveList => SpeciesData.LevelUpMoves;

    public void ResetMoves()
    {
        var thisLevel = Level;
        var moveset = MoveList;

        var knowableMoves = moveset.Where(m => m.Level <= thisLevel).Select(m => m.Move).ToList();

        // Remove duplicates (retaining the latest copy of each move)
        knowableMoves.Reverse();
        knowableMoves.DistinctInPlace();
        knowableMoves.Reverse();

        Moves.Clear();
        var firstMoveIndex = Math.Max(knowableMoves.Count - MaxMoves, 0);
        for (var i = firstMoveIndex; i < knowableMoves.Count; i++)
        {
            Moves.Add(new PokemonMove(knowableMoves[i]));
        }
    }

    public void LearnMove(Name moveId)
    {
        for (var i = 0; i < Moves.Count; i++)
        {
            var move = Moves[i];
            if (move.Id != moveId)
                continue;

            Moves.Add(move);
            Moves.RemoveAt(i);
            return;
        }

        Moves.Add(new PokemonMove(moveId));
        if (Moves.Count > MaxMoves)
            Moves.RemoveAt(0);
    }

    public void ForgetMove(Name moveId)
    {
        Moves.RemoveAll(m => m.Id == moveId);
    }

    public void ForgetMove(int moveIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(moveIndex);
        Moves.RemoveAt(moveIndex);
    }

    public void ForgetAllMoves() => Moves.Clear();

    public List<Name> FirstMoves { get; set; } = [];

    public void RecordFirstMoves()
    {
        ClearFirstMoves();
        FirstMoves.AddRange(Moves.Select(m => m.Id));
    }

    public void AddFirstMove(Name moveId)
    {
        if (!FirstMoves.Contains(moveId))
            FirstMoves.Add(moveId);
    }

    public void RemoveFirstMove(Name moveId) => FirstMoves.Remove(moveId);

    public void ClearFirstMoves() => FirstMoves.Clear();

    public bool CompatibleWithMove(Name moveId)
    {
        var speciesData = SpeciesData;
        return SpeciesData
            .TutorMoves.Concat(MoveList.Select(m => m.Move))
            .Concat(speciesData.EggMoves)
            .Contains(moveId);
    }

    [IgnoreMember]
    public bool CanRelearnMove => GameGlobal.MoveService.CanRelearnMoves(this);

    #endregion

    #region Ribbons
    public List<Name> Ribbons { get; set; } = [];

    [IgnoreMember]
    public int RibbonCount => Ribbons.Count;

    public bool HasRibbon(Name ribbonId) => Ribbons.Contains(ribbonId);

    public void GiveRibbon(Name ribbonId)
    {
        if (!HasRibbon(ribbonId))
            Ribbons.Add(ribbonId);
    }

    public Name? UpgradeRibbon(params ReadOnlySpan<Name> ribbonIds)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ribbonIds.Length);
        foreach (var (i, ribbonId) in ribbonIds.AsValueEnumerable().Index())
        {
            for (var j = 0; j < Ribbons.Count; j++)
            {
                if (Ribbons[j] != ribbonId || i == ribbonIds.Length - 1)
                    continue;

                Ribbons[j] = ribbonIds[i + 1];
                return Ribbons[j];
            }
        }

        if (HasRibbon(ribbonIds[^1]))
            return null;

        GiveRibbon(ribbonIds[0]);
        return ribbonIds[0];
    }

    public bool TakeRibbon(Name ribbonId)
    {
        return Ribbons.Remove(ribbonId);
    }

    public void ClearAllRibbons() => Ribbons.Clear();

    #endregion

    #region Pokerus

    public int Pokerus { get; set; }

    [IgnoreMember]
    public int PokerusStrain => Pokerus / 16;

    [IgnoreMember]
    public PokerusStage PokerusStage
    {
        get
        {
            if (Pokerus == 0)
                return PokerusStage.NotInfected;

            return Pokerus % 16 == 0 ? PokerusStage.Cured : PokerusStage.Infected;
        }
    }

    public void GivePokerus(int strain = 0)
    {
        if (PokerusStage == PokerusStage.Cured)
            return;

        GameGlobal.GameStats.PokerusInfections++;

        if (strain is <= 0 or >= 16)
        {
            strain = Random.Shared.Next(1, 16);
        }

        SetPokerusTime(strain);
    }

    public void ResetPokerusTime()
    {
        if (Pokerus == 0)
            return;

        var strain = Pokerus / 16;
        SetPokerusTime(strain);
    }

    private void SetPokerusTime(int strain)
    {
        var time = 1 + strain % 4;
        Pokerus = time;
        Pokerus |= strain << 4;
    }

    public void LowerPokerusCount()
    {
        if (PokerusStage != PokerusStage.Infected)
            return;
        Pokerus--;
    }

    public void CurePokerus()
    {
        Pokerus -= Pokerus % 16;
    }

    #endregion

    # region Ownership, obtained information

    public PokemonOwner Owner { get; set; }

    [IgnoreMember]
    public bool IsForeign => IsForeignTo(GameGlobal.PlayerTrainer);

    public bool IsForeignTo(Trainer trainer)
    {
        return Owner.Id != trainer.Id || Owner.Name != trainer.Name;
    }

    public ObtainMethod ObtainMethod { get; set; }

    public int ObtainMap { get; set; }

    public DateTimeOffset TimeReceived { get; set; }

    public Text ObtainText { get; set; }

    public int ObtainLevel { get; set; }

    public Name HatchedMap { get; set; }

    public DateTimeOffset TimeEggHatched { get; set; }

    #endregion

    #region Other

    private Text? _name;

    [IgnoreMember]
    public Text Name
    {
        get => _name ?? SpeciesName;
        set
        {
            if (value.AsReadOnlySpan().IsEmpty || value == SpeciesName)
            {
                _name = null;
                return;
            }

            _name = value;
        }
    }

    [MemberNotNullWhen(true, nameof(_name))]
    [IgnoreMember]
    public bool IsNicknamed => _name.HasValue && !_name.Value.AsReadOnlySpan().IsEmpty;

    [IgnoreMember]
    public Text SpeciesName => SpeciesData.Name;

    [IgnoreMember]
    public int Height => SpeciesData.Height;

    [IgnoreMember]
    public int Weight => SpeciesData.Weight;

    [IgnoreMember]
    public IReadOnlyDictionary<Name, int> EVYield => SpeciesData.EVs;

    public int Happiness { get; set; }

    [IgnoreMember]
    public int AffectionLevel
    {
        get
        {
            return Happiness switch
            {
                >= 0 and < 100 => 0,
                >= 100 and < 150 => 1,
                >= 150 and < 200 => 2,
                >= 200 and < 230 => 3,
                >= 230 and < 255 => 4,
                _ => 5,
            };
        }
    }

    public void ChangeHappiness(HappinessChangeMethod method)
    {
        HappinessChangeService.Instance.ApplyHappinessChange(this, method);
    }

    #endregion

    #region Evolution Checks

    public Name? CheckEvolutionOnLevelUp()
    {
        var evolutionService = GameGlobal.EvolutionService;
        return CheckEvolutionInternal(
            (pkmn, newSpecies, method, parameter) =>
                evolutionService.OnLevelUp(method, pkmn, parameter) ? newSpecies : (Name?)null
        );
    }

    public Name? CheckEvolutionOnUseItem(Name itemUsed)
    {
        var evolutionService = GameGlobal.EvolutionService;
        return CheckEvolutionInternal(
            (pkmn, newSpecies, method, parameter) =>
                evolutionService.OnUseItem(method, pkmn, parameter, itemUsed) ? newSpecies : (Name?)null
        );
    }

    public Name? CheckEvolutionOnTrade(Pokemon otherPkmn)
    {
        var evolutionService = GameGlobal.EvolutionService;
        return CheckEvolutionInternal(
            (pkmn, newSpecies, method, parameter) =>
                evolutionService.OnTrade(method, pkmn, parameter, otherPkmn) ? newSpecies : (Name?)null
        );
    }

    public Name? CheckEvolutionAfterBattle(int partyIndex)
    {
        var evolutionService = GameGlobal.EvolutionService;
        return CheckEvolutionInternal(
            (pkmn, newSpecies, method, parameter) =>
                evolutionService.AfterBattle(method, pkmn, partyIndex, parameter) ? newSpecies : (Name?)null
        );
    }

    public Name? CheckEvolutionByEvent(object? value)
    {
        var evolutionService = GameGlobal.EvolutionService;
        return CheckEvolutionInternal(
            (pkmn, newSpecies, method, parameter) =>
                evolutionService.OnEvent(method, pkmn, parameter, value) ? newSpecies : (Name?)null
        );
    }

    public void ActionAfterEvolution(Name newSpecies)
    {
        var evolutionService = GameGlobal.EvolutionService;
        foreach (var (evoSpecies, method, parameter, _) in SpeciesData.GetEvolutions(true))
        {
            if (evolutionService.AfterEvolution(method, this, evoSpecies, parameter, newSpecies))
                break;
        }
    }

    private Name? CheckEvolutionInternal(Func<Pokemon, Name, Name, object?, Name?> selector)
    {
        if (!GameGlobal.EvolutionService.CanEvolve(this))
            return null;

        foreach (var (newSpecies, method, parameter, _) in SpeciesData.GetEvolutions().Where(e => !e.IsPrevious))
        {
            var result = selector(this, newSpecies, method, parameter);
            if (result != null)
                return result;
        }

        return null;
    }

    public async ValueTask<bool> TriggerEvolutionByEvent(object? value)
    {
        var newSpecies = CheckEvolutionByEvent(value);
        if (!newSpecies.HasValue)
            return false;

        await GameGlobal.EngineInteropService.FadeOutInWithMusic(async () =>
            await IScreenActionService.Instance.EvolvePokemonScreen(this, newSpecies.Value)
        );

        return true;
    }

    #endregion

    #region Stat Calculation

    public Dictionary<Name, int> IV { get; set; } = new();

    public Dictionary<Name, bool> IVMaxed { get; set; } = new();

    public Dictionary<Name, int> EV { get; set; } = new();

    public int MaxHP { get; private set; }

    public int Attack { get; private set; }

    public int Defense { get; private set; }

    public int SpecialAttack { get; private set; }

    public int SpecialDefense { get; private set; }

    public int Speed { get; private set; }

    [IgnoreMember]
    public IReadOnlyDictionary<Name, int> BaseStats => SpeciesData.BaseStats;

    private ImmutableDictionary<Name, int> CalcIV()
    {
        return Stat.AllMain.ToImmutableDictionary(
            s => s.Id,
            s => IVMaxed.TryGetValue(s.Id, out var maxed) && maxed ? IVStatLimit : IV[s.Id]
        );
    }

    private static int CalcHP(int baseValue, int level, int iv, int ev)
    {
        if (baseValue == 1)
            return 1;

        // ReSharper disable once InvertIf
        if (GameGlobal.GameSettings.DisableIVsAndEVs)
        {
            iv = 0;
            ev = 0;
        }

        return (baseValue * 2 + iv + ev / 4) * level / 100 + level + 10;
    }

    private static int CalcStat(int baseValue, int level, int iv, int ev, int natureChange)
    {
        // ReSharper disable once InvertIf
        if (GameGlobal.GameSettings.DisableIVsAndEVs)
        {
            iv = 0;
            ev = 0;
        }

        return ((baseValue * 2 + iv + ev / 4) * level / 100 + 5) * natureChange / 100;
    }

    public void CalcStats()
    {
        if (IV.Count == 0 || EV.Count == 0)
            return;

        var baseStats = BaseStats;
        var thisLevel = Level;
        var thisIV = CalcIV();

        var natureMod = Stat.AllMain.ToDictionary(s => s.Id, _ => 100);
        var thisNature = NatureForStats;
        if (thisNature is not null)
        {
            foreach (var (statId, change) in thisNature.StatChanges)
            {
                natureMod[statId] += change;
            }
        }

        var stats = Stat.AllMain.ToImmutableDictionary(
            s => s.Id,
            s =>
                s.Id == Stat.HP.Id
                    ? CalcHP(baseStats[s.Id], thisLevel, thisIV[s.Id], EV[s.Id])
                    : CalcStat(baseStats[s.Id], thisLevel, thisIV[s.Id], EV[s.Id], natureMod[s.Id])
        );

        var hpDifference = stats[Stat.HP.Id] - MaxHP;
        MaxHP = stats[Stat.HP.Id];
        if (HP > 0 || hpDifference > 0)
        {
            HP = Math.Max(HP + hpDifference, 1);
        }

        Attack = stats[Stat.Attack.Id];
        Defense = stats[Stat.Defense.Id];
        SpecialAttack = stats[Stat.SpecialAttack.Id];
        SpecialDefense = stats[Stat.SpecialDefense.Id];
        Speed = stats[Stat.Speed.Id];
    }

    #endregion

    #region Other Data Members

    public int Cool { get; set; }

    public int Beauty { get; set; }

    public int Cute { get; set; }

    public int Smart { get; set; }

    public int Tough { get; set; }

    public int Sheen { get; set; }

    public Name PokeBall { get; set; }

    public List<int> Markings { get; set; } = [];

    public Pokemon? Fused { get; set; }

    public uint PersonalityValue { get; set; }

    #endregion

    #region Tags

    private readonly HashSet<Name> _tags = [];

    public void AddTag(Name tag)
    {
        _tags.Add(tag);
    }

    public void RemoveTag(Name tag)
    {
        _tags.Remove(tag);
    }

    public bool HasTag(Name tag)
    {
        return _tags.Contains(tag);
    }

    public bool HasAnyTag(params ReadOnlySpan<Name> tags)
    {
        return tags.AsValueEnumerable().Any(HasTag);
    }

    public bool HasAllTags(params ReadOnlySpan<Name> tags)
    {
        return tags.AsValueEnumerable().All(HasTag);
    }

    #endregion

    #region Components

    [MessagePackFormatter(typeof(PokemonComponentsMessagePackFormatter))]
    private Dictionary<Name, IPokemonComponent> _components;

    [IgnoreMember]
    public IEnumerable<IPokemonComponent> Components => _components.Values;

    public IPokemonComponent? GetComponent(Name componentId)
    {
        return _components.GetValueOrDefault(componentId);
    }

    public T? GetComponent<T>()
        where T : class, IPokemonComponent<T>
    {
        return _components.GetValueOrDefault(T.ComponentId) as T;
    }

    #endregion

    #region Pokémon Creation

    public Pokemon Clone()
    {
        var result = (Pokemon)MemberwiseClone();
        result.IV = new Dictionary<Name, int>(IV);
        result.IVMaxed = new Dictionary<Name, bool>(IVMaxed);
        result.EV = new Dictionary<Name, int>(EV);
        result.Moves = new List<PokemonMove>(Moves.Select(m => m.Clone()));
        result.FirstMoves = new List<Name>(FirstMoves);
        result.Ribbons = new List<Name>(Ribbons);
        result._components = _components.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Clone(result));
        return result;
    }

    private static readonly Name PokeBallName = "POKEBALL";

    [SerializationConstructor]
    // ReSharper disable once InconsistentNaming
    private Pokemon(PokemonOwner owner, Dictionary<Name, IPokemonComponent> _components)
    {
        Owner = owner;
        this._components = _components;
        foreach (var component in _components.Values)
        {
            component.Attach(this);
        }
    }

    public Pokemon(SpeciesForm species, int level, PokemonOwner owner, bool withMoves = true, bool recheckForm = true)
    {
        var speciesData = Data.Pbs.Species.Get(species);
        _species = speciesData.SpeciesId;
        _form = speciesData.BaseForm;
        Level = level;
        if (withMoves)
            ResetMoves();
        Happiness = speciesData.Happiness;
        PokeBall = PokeBallName;

        foreach (var stat in Stat.AllMain)
        {
            IV.Add(stat.Id, Random.Shared.Next(IVStatLimit + 1));
            EV.Add(stat.Id, 0);
        }

        Owner = owner;
        ObtainMap = GameGlobal.GameMap.MapId;
        ObtainLevel = level;
        TimeReceived = DateTimeOffset.Now;

        PersonalityValue = (uint)(Random.Shared.Next(0x10000) | (Random.Shared.Next(0x10000) << 16));
        MaxHP = 1;
        HP = 1;
        CalcStats();

        if (_form == 0 && recheckForm)
        {
            var form = MultipleForms.GetFormOnCreation(this);
            if (form.HasValue)
            {
                Form = form.Value;
                if (withMoves)
                    ResetMoves();
            }
        }

        _components = GameGlobal.PokemonComponentService.CreateComponents(this).ToDictionary(c => c.Id);
    }

    public Pokemon(
        SpeciesForm species,
        int level,
        Trainer? owner = null,
        bool withMoves = true,
        bool recheckForm = true
    )
        : this(
            species,
            level,
            owner is not null
                ? PokemonOwner.FromNewTrainer(owner)
                : PokemonOwner.FromNewTrainer(GameGlobal.PlayerTrainer),
            withMoves,
            recheckForm
        ) { }

    #endregion
}
