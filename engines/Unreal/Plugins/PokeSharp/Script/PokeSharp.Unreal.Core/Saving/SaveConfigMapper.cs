using PokeSharp.Core.Saving;
using PokeSharp.Unreal.Core.Configuration;
using Riok.Mapperly.Abstractions;
using UnrealSharp.PokeSharpCore;

namespace PokeSharp.Unreal.Core.Saving;

[RegisterSingleton]
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SaveConfigMapper : IUnrealOptionsMapper<UPokeSharpSaveSettings, SaveDataConfig>
{
    [MapProperty(nameof(UPokeSharpSaveSettings.SaveSlotName), nameof(SaveDataConfig.SaveFileName))]
    [MapperIgnoreTarget(nameof(SaveDataConfig.SaveFilePath))]
    public partial SaveDataConfig Map(UPokeSharpSaveSettings settings);
}
