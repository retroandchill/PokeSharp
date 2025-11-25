using UnrealBuildTool;

public class PokeSharp : ModuleRules
{
    public PokeSharp(ReadOnlyTargetRules target)
        : base(target)
    {
        PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;

        PublicDependencyModuleNames.AddRange([
            "Core",
            "DeveloperSettings",
            "UMG",
            "CommonUI",
            "UnrealSharpBinds",
            "UnrealSharpCore",
            "UnrealSharpAsync",
        ]);

        PrivateDependencyModuleNames.AddRange([
            "CoreUObject",
            "Engine",
            "Slate",
            "SlateCore",
            "CommonUtilities",
            "CommonInput",
            "GameplayTags",
            "EnhancedInput",
        ]);
    }
}
