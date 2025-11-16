// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Compiler.Core;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using PokeSharp.Data;
using PokeSharp.Settings;

var configurationBuilder = new ConfigurationBuilder();

var configuration = configurationBuilder.Build();

configuration.Get<GameSettings>();

var builder = new GameContextBuilder();

builder
    .Services.AddLogging()
    .AddPokeSharpCore(SerializerTags.MessagePack)
    .AddPokeSharpCompilerCore()
    .AddPokeSharp()
    .AddPokeSharpCompiler();

var context = builder.Build();

try
{
    GameContext.Initialize(context);

    var compilerService = context.GetService<PbsCompilerService>();

    await compilerService.CompilePbsFilesAsync();

    await compilerService.WritePbsFilesAsync();
}
finally
{
    GameContext.Reset();
}
