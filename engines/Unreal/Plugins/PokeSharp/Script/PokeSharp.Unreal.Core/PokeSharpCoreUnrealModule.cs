using PokeSharp.Core;
using UnrealSharp.Engine.Core.Modules;

namespace PokeSharp.Unreal.Core;

public class FPokeSharpCoreUnrealModule : IModuleInterface
{
    public void StartupModule()
    {
        INameProvider.UseCustomProvider(new UnrealNameProvider());
        ITextProvider.UseCustomProvider(new UnrealTextProvider());
    }

    public void ShutdownModule()
    {
        INameProvider.ResetNameProvider();
        ITextProvider.ResetTextProvider();
    }
}
