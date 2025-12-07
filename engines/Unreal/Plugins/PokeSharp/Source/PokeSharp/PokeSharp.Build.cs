using UnrealBuildTool;

public class PokeSharp : ModuleRules
{
    public PokeSharp(ReadOnlyTargetRules target)
        : base(target)
    {
        PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;
        CppStandard = CppStandardVersion.Cpp23;

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
