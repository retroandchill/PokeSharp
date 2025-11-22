using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UnrealSharp.DeveloperSettings;

namespace PokeSharp.Core.Unreal.Configuration;

public static class UnrealOptionsExtensions
{
    public static IServiceCollection AddUnrealOptions<TUnrealSettings, TOptions>(this IServiceCollection services)
        where TUnrealSettings : UDeveloperSettings
        where TOptions : class
    {
        services.AddSingleton<
            IOptionsChangeTokenSource<TOptions>,
            UnrealOptionsChangeTokenSource<TUnrealSettings, TOptions>
        >();
        services.AddTransient<IOptionsFactory<TOptions>, UnrealOptionsFactory<TUnrealSettings, TOptions>>();
        return services;
    }
}
