// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Compiler.Core;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using PokeSharp.Data;

var builder = new GameContextBuilder("test");

builder
    .Services.AddLogging()
    .AddPokeSharpCore(SerializerTags.MessagePack)
    .AddPokeSharpCompilerCore()
    .AddPokeSharp()
    .AddPokeSharpCompiler();

var context = builder.Build();

try
{
    GameContextManager.Initialize(context);

    DefaultData.AddAll();

    var compilerService = context.GetService<PbsCompilerService>();

    await compilerService.CompilePbsFilesAsync();
}
finally
{
    GameContextManager.Reset();
}
