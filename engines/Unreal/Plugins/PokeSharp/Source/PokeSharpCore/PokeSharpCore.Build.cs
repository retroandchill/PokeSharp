using UnrealBuildTool;

public class PokeSharpCore : ModuleRules
{
    public PokeSharpCore(ReadOnlyTargetRules target)
        : base(target)
    {
        PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;
        CppStandard = CppStandardVersion.Cpp23;

        PublicDependencyModuleNames.AddRange(
            [
                "Core",
                "UnrealSharpBinds",
                "UnrealSharpCore",
                "DeveloperSettings",
                "UnrealSharpAsync",
                "CommonUI",
                "CommonInput",
            ]
        );

        PrivateDependencyModuleNames.AddRange(
            ["CoreUObject", "Engine", "Slate", "SlateCore", "UMG", "CommonUtilities", "GameplayTags"]
        );
    }
}
