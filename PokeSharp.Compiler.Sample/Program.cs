// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Compiler.Compilers;
using PokeSharp.Compiler.Core;
using PokeSharp.Core;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;
using Environment = PokeSharp.Data.Core.Environment;

var builder = new GameContextBuilder("test");

builder
    .Services.AddLogging()
    .AddSingleton<PbsCompilerService>()
    .AddSingleton<IPbsCompiler, TypeCompiler>()
    .AddSingleton<IPbsCompiler, AbilityCompiler>()
    .AddSingleton<IPbsCompiler, MoveCompiler>()
    .AddSingleton<IPbsCompiler, ItemCompiler>()
    .AddSingleton<IPbsCompiler, BerryPlantCompiler>()
    .AddSingleton<IPbsCompiler, PokemonCompiler>()
    .AddSingleton<IPbsCompiler, PokemonFormCompiler>()
    .AddSingleton<IPbsCompiler, ShadowPokemonCompiler>()
    .AddSingleton<IPbsCompiler, RibbonCompiler>()
    .AddSingleton<IPbsCompiler, TrainerTypeCompiler>()
    .AddGrowthRateData()
    .AddGenderRatioData()
    .AddEggGroupData()
    .AddBodyShapeData()
    .AddBodyColorData()
    .AddHabitatData()
    .AddEvolutionData()
    .AddStatData()
    .AddNatureData()
    .AddWeatherData()
    .AddEncounterTypeData()
    .AddEnvironmentData()
    .AddBattleWeatherData()
    .AddBattleTerrainData()
    .AddTargetData()
    .AddPokemonTypeData()
    .AddAbilityData()
    .AddMoveData()
    .AddItemData()
    .AddBerryPlantData()
    .AddSpeciesData()
    .AddShadowPokemonData()
    .AddRibbonData()
    .AddTrainerTypeData()
    .AddTrainerData();

var context = builder.Build();

try
{
    GameContextManager.Initialize(context);

    GrowthRate.AddDefaultValues();
    GenderRatio.AddDefaultValues();
    EggGroup.AddDefaultValues();
    BodyShape.AddDefaultValues();
    BodyColor.AddDefaultValues();
    Habitat.AddDefaultValues();
    Evolution.AddDefaultValues();
    Stat.AddDefaultValues();
    Nature.AddDefaultValues();
    Weather.AddDefaultValues();
    EncounterType.AddDefaultValues();
    Environment.AddDefaultValues();
    BattleWeather.AddDefaultValues();
    BattleTerrain.AddDefaultValues();
    Target.AddDefaultValues();

    var compilerService = context.GetService<PbsCompilerService>();

    await compilerService.CompilePbsFiles();
    await compilerService.WritePbsFiles();
}
finally
{
    GameContextManager.Reset();
}
