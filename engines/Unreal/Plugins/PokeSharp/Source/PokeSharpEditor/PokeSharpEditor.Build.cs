using UnrealBuildTool;

public class PokeSharpEditor : ModuleRules
{
    public PokeSharpEditor(ReadOnlyTargetRules target)
        : base(target)
    {
        PCHUsage = ModuleRules.PCHUsageMode.UseExplicitOrSharedPCHs;

        PublicDependencyModuleNames.AddRange(["Core", "UnrealSharpBinds", "UnrealSharpCore", "CommonUtilities"]);

        PrivateDependencyModuleNames.AddRange(["CoreUObject", "Engine", "Slate", "SlateCore", "Json"]);
    }
}
