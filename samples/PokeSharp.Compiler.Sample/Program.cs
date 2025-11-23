// See https://aka.ms/new-console-template for more information

using System.Collections.Immutable;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeSharp;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Sample;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using PokeSharp.Core.Saving;
using PokeSharp.Maps;
using PokeSharp.PokemonModel;
using PokeSharp.Trainers;

var builder = new GameContextBuilder();

builder
    .Services.AddLogging(logging => logging.AddConsole())
    .AddSingleton<IFileSystem, FileSystem>()
    .AddPokeSharpCore()
    .AddPokeSharpCompilerCore()
    .AddPokeSharp()
    .AddPokeSharpCompiler()
    .AddPokeSharpRGSS();

var context = builder.Build();

try
{
    GameContext.Initialize(context);

    var mapMetadataRepository = context.GetService<IMapMetadataRepository>();
    await mapMetadataRepository.LoadAsync();

    var compilerService = context.GetService<PbsCompilerService>();
    await compilerService.RunCompileOnStartAsync();

    var gameState = context.GetService<GameState>();
    await gameState.InitializeAsync();
    await gameState.SetUpSystemAsync();
    await gameState.StartNewAsync();

    var player = GameGlobal.PlayerTrainer;
    player.Name = "Red";

    ImmutableArray<Name> startingSpecies = ["PIKACHU", "AXEW", "SNIVY", "OSHAWOTT", "TEPIG", "RIOLU"];
    foreach (var species in startingSpecies)
    {
        player.Party.Add(new Pokemon(species, 10));
    }

    player.PrintParty();

    var saveService = context.GetService<SaveDataService>();
    await saveService.SaveToFileAsync("test.sav");

    var saveData = await saveService.ReadDataFromFileAsync("test.sav");
    saveService.LoadAllValues(saveData);
    player = GameGlobal.PlayerTrainer;
    Console.WriteLine(player.Name);
    player.PrintParty();
}
finally
{
    GameContext.Reset();
}
