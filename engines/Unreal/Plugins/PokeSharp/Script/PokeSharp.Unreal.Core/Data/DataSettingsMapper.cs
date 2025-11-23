using System.Runtime.CompilerServices;
using PokeSharp.Core.Data;
using PokeSharp.Unreal.Core.Configuration;
using Riok.Mapperly.Abstractions;
using UnrealSharp.CoreUObject;
using UnrealSharp.PokeSharpCore;

namespace PokeSharp.Unreal.Core.Data;

[RegisterSingleton]
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class DataSettingsMapper : IUnrealOptionsMapper<UPokeSharpDataSettings, DataSettings>
{
    [MapProperty(nameof(UPokeSharpDataSettings.DataPath), nameof(DataSettings.DataFileBasePath))]
    public partial DataSettings Map(UPokeSharpDataSettings settings);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string Map(FDirectoryPath path) => path.Path;
}
