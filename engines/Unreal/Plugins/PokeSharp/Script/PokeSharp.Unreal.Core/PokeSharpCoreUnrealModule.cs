using PokeSharp.Core;
using PokeSharp.Core.Strings;
using PokeSharp.Unreal.Core.Strings;
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
