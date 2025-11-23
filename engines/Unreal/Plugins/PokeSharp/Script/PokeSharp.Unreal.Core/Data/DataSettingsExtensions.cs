using Microsoft.Extensions.DependencyInjection;
using PokeSharp.Core.Data;
using PokeSharp.Unreal.Core.Configuration;
using UnrealSharp.PokeSharpCore;

namespace PokeSharp.Unreal.Core.Data;

public static class DataSettingsExtensions
{
    [RegisterServices]
    public static void RegisterDataSettings(this IServiceCollection services)
    {
        services.AddUnrealOptions<UPokeSharpDataSettings, DataSettings>();
    }
}
