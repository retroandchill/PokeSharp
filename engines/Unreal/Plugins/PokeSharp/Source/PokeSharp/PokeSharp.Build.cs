using UnrealBuildTool;

public class PokeSharp : ModuleRules
{
    public PokeSharp(ReadOnlyTargetRules Target)
        : base(Target)
    {
        PCHUsage = ModuleRules.PCHUsageMode.UseExplicitOrSharedPCHs;

        PublicDependencyModuleNames.AddRange(new string[] { "Core", "DeveloperSettings" });

        PrivateDependencyModuleNames.AddRange(new string[] { "CoreUObject", "Engine", "Slate", "SlateCore" });
    }
}
