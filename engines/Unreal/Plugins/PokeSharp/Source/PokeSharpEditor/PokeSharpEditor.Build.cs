using UnrealBuildTool;

public class PokeSharpEditor : ModuleRules
{
    public PokeSharpEditor(ReadOnlyTargetRules target)
        : base(target)
    {
        PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;
        CppStandard = CppStandardVersion.Cpp23;

        PublicDependencyModuleNames.AddRange(
            [
                "Core",
                "UnrealSharpBinds",
                "UnrealSharpCore",
                "CommonUtilities",
                "Json",
                "Slate",
                "Mcro",
                "PropertyEditor",
            ]
        );

        PrivateDependencyModuleNames.AddRange(
            ["CoreUObject", "Engine", "Slate", "SlateCore", "ToolMenus", "Projects", "InputCore", "JsonUtilities"]
        );
    }
}
