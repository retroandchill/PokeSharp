using Injectio.Attributes;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Data;

public static class PbsDataRegistration
{
    [RegisterServices]
    public static void RegisterPbsRepositories(IServiceCollection services)
    {
        services
            .RegisterGameDataRepository<Ability, Name>()
            .RegisterGameDataRepository<BerryPlant, Name>()
            .RegisterGameDataRepository<Encounter, EncounterId>()
            .RegisterGameDataRepository<EnemyTrainer, TrainerIdentifier>()
            .RegisterGameDataRepository<Item, Name>()
            .RegisterGameDataRepository<Metadata, int>()
            .RegisterGameDataRepository<MapConnection, int>()
            .RegisterGameDataRepository<RegionalDex, int>()
            .RegisterGameDataRepository<Move, Name>()
            .RegisterGameDataRepository<PlayerMetadata, int>()
            .RegisterGameDataRepository<PokemonType, Name>()
            .RegisterGameDataRepository<Ribbon, Name>()
            .RegisterGameDataRepository<ShadowPokemon, SpeciesForm>()
            .RegisterGameDataRepository<Species, SpeciesForm>()
            .RegisterGameDataRepository<SpeciesMetrics, SpeciesForm>()
            .RegisterGameDataRepository<TownMap, int>()
            .RegisterGameDataRepository<TrainerType, Name>();
    }
}
