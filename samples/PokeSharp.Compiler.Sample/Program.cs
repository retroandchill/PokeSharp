// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokeSharp;
using PokeSharp.Compiler.Core;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using PokeSharp.Maps;

var builder = new GameContextBuilder();

builder
    .Services.AddLogging(logging => logging.AddConsole())
    .AddPokeSharpCore(SerializerTags.MessagePack)
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
    
    await compilerService.WritePbsFilesAsync();
}
finally
{
    GameContext.Reset();
}
