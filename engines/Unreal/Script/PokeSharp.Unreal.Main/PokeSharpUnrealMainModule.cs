using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Core;
using UnrealSharp.Engine.Core.Modules;

namespace PokeSharp.Unreal.Main;

public class FPokeSharpUnrealMainModule : IModuleInterface
{
    public void StartupModule()
    {
        var builder = new GameContextBuilder();
        builder
            .Services.AddPokeSharpCore()
            .AddPokeSharpAbstractions()
            .AddPokeSharp()
            .AddPokeSharpUnrealCore()
            .AddPokeSharpUnreal();

        var context = builder.Build();
        GameContext.Initialize(context);

        var gameState = context.GetService<GameState>();
        gameState.Initialize();
        gameState.SetUpSystem();
    }

    public void ShutdownModule()
    {
        GameContext.Reset();
    }
}
