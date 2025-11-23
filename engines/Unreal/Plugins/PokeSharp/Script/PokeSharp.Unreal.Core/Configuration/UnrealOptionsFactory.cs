using Microsoft.Extensions.Options;
using UnrealSharp.CoreUObject;
using UnrealSharp.DeveloperSettings;

namespace PokeSharp.Unreal.Core.Configuration;

public class UnrealOptionsFactory<TUnrealSettings, TOptions>(IUnrealOptionsMapper<TUnrealSettings, TOptions> mapper)
    : IOptionsFactory<TOptions>
    where TUnrealSettings : UDeveloperSettings
    where TOptions : class
{
    public TOptions Create(string name)
    {
        var settings = UObject.GetDefault<TUnrealSettings>();
        return mapper.Map(settings);
    }
}
