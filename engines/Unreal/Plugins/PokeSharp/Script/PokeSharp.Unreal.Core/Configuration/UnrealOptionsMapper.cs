using UnrealSharp.DeveloperSettings;

namespace PokeSharp.Unreal.Core.Configuration;

public interface IUnrealOptionsMapper<in TUnrealSettings, out TOptions>
    where TUnrealSettings : UDeveloperSettings
    where TOptions : class
{
    TOptions Map(TUnrealSettings settings);
}
