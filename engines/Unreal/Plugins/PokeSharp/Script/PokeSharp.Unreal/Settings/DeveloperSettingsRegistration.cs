using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Settings;
using PokeSharp.Unreal.Core.Configuration;
using UnrealSharp.PokeSharp;

namespace PokeSharp.Unreal.Settings;

public static class DeveloperSettingsRegistration
{
    [RegisterServices]
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddUnrealOptions<UPokeSharpSettings, GameSettings>();
    }
}
