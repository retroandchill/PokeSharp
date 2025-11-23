using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using UnrealSharp.DeveloperSettings;

namespace PokeSharp.Unreal.Core.Configuration;

public class UnrealOptionsChangeTokenSource<TUnrealSettings, TOptions> : IOptionsChangeTokenSource<TOptions>
    where TUnrealSettings : UDeveloperSettings
    where TOptions : class
{
    public string Name => Options.DefaultName;

    public IChangeToken GetChangeToken() => new UnrealChangeToken<TUnrealSettings>();
}
