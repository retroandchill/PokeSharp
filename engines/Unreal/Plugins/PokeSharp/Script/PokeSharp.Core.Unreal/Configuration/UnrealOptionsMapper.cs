using UnrealSharp.DeveloperSettings;

namespace PokeSharp.Core.Unreal.Configuration;

public interface IUnrealOptionsMapper<in TUnrealSettings, out TOptions>
    where TUnrealSettings : UDeveloperSettings
    where TOptions : class
{
    TOptions Map(TUnrealSettings settings);
}
