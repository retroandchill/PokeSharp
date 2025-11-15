using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Core;
using PokeSharp.Core.Engine;
using PokeSharp.Core.Settings;
using PokeSharp.Data.Pbs;
using PokeSharp.PokemonModel;

namespace PokeSharp.Trainers;

public abstract class Trainer(Text name, Name trainerType)
{
    public Name TrainerType { get; set; } = trainerType;

    public Text Name { get; set; } = name;

    public uint Id { get; set; } = (uint)Random.Shared.NextInt64(uint.MaxValue);

    public Name Language { get; set; } = GameServices.LocalizationService.Language;

    public List<Pokemon> Party { get; set; } = [];

    private static readonly Name NoNameFlag = "NoName";

    public Text FullName => HasFlag(NoNameFlag) ? Name : $"{TrainerType} {Name}";

    public uint PublicId => GetPublicId(Id);

    private const uint PublicIdMax = 1000000;

    public static uint GetPublicId(uint id)
    {
        return id % PublicIdMax;
    }

    public uint SecretId => GetSecretId(Id);

    public static uint GetSecretId(uint id)
    {
        return id / PublicIdMax;
    }

    public uint MakeForeignId()
    {
        uint newId;
        do
        {
            newId = (uint)Random.Shared.NextInt64(uint.MaxValue);
        } while (newId == Id);

        return newId;
    }

    public Text TrainerTypeName => Data.Pbs.TrainerType.Get(TrainerType).Name;

    public int BaseMoney => Data.Pbs.TrainerType.Get(TrainerType).BaseMoney;

    public TrainerGender Gender => Data.Pbs.TrainerType.Get(TrainerType).Gender;

    public bool IsMale => Data.Pbs.TrainerType.Get(TrainerType).IsMale;

    public bool IsFemale => Data.Pbs.TrainerType.Get(TrainerType).IsFemale;

    public int SkillLevel => Data.Pbs.TrainerType.Get(TrainerType).SkillLevel;

    public ImmutableArray<Name> Flags => Data.Pbs.TrainerType.Get(TrainerType).Flags;

    public bool HasFlag(Name flag) => Data.Pbs.TrainerType.Get(TrainerType).HasFlag(flag);

    public IEnumerable<Pokemon> PokemonParty => Party.Where(p => !p.IsEgg);

    public IEnumerable<Pokemon> AbleParty => Party.Where(p => p is { IsEgg: false, IsFainted: false });

    public int PartyCount => Party.Count;

    public int AblePokemonCount => AbleParty.Count();

    public bool IsPartyFull => PartyCount >= GameServices.GameSettings.MaxPartySize;

    public bool AllFainted => AblePokemonCount == 0;

    public Pokemon? FirstPartyMember => Party.Count > 0 ? Party[0] : null;

    public Pokemon? FirstAblePokemon => AbleParty.FirstOrDefault();

    public Pokemon? LastPartyMember => Party.Count > 0 ? Party[^1] : null;

    public Pokemon? LastPokemon
    {
        get
        {
            var party = PokemonParty.ToArray();
            return party.Length > 0 ? party[^1] : null;
        }
    }

    public Pokemon? LastAblePokemon
    {
        get
        {
            var party = AbleParty.ToArray();
            return party.Length > 0 ? party[^1] : null;
        }
    }

    public bool RemovePokemonAtIndex(int index)
    {
        if (index < 0 || index >= PartyCount || !HasOtherAblePokemon(index))
            return false;

        Party.RemoveAt(index);
        return true;
    }

    public bool HasOtherAblePokemon(int index)
    {
        return Party.Index().Any(x => x.Index != index && x.Item.IsAble);
    }

    public bool HasSpecies(Name species, int? form = null) =>
        PokemonParty.Any(p => p.IsSpecies(species) && (!form.HasValue || form.Value == p.Form));

    public bool HasFatefulSpecies(Name species) =>
        PokemonParty.Any(p => p.IsSpecies(species) && p.ObtainMethod == ObtainMethod.FatefulEncounter);

    public bool HasPokemonOfType(Name type) => PokemonParty.Any(p => p.HasType(type));

    public Pokemon? GetPokemonWithMove(Name move)
    {
        return PokemonParty.FirstOrDefault(pkmn => pkmn.HasMove(move));
    }

    public void HealParty()
    {
        foreach (var pkmn in Party)
        {
            pkmn.Heal();
        }
    }
}
