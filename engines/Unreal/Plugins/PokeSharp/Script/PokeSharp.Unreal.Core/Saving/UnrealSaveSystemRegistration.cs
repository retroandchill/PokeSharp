using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Core.Saving;
using PokeSharp.Unreal.Core.Configuration;
using UnrealSharp.PokeSharpCore;

namespace PokeSharp.Unreal.Core.Saving;

public static class UnrealSaveSystemRegistration
{
    [RegisterServices]
    public static void RegisterSaveSystem(this IServiceCollection services)
    {
        services.AddUnrealOptions<UPokeSharpSaveSettings, SaveDataConfig>();
    }
}
