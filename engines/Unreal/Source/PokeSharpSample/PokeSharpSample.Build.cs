// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;

public class PokeSharpSample : ModuleRules
{
    public PokeSharpSample(ReadOnlyTargetRules target)
        : base(target)
    {
        PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;

        PublicDependencyModuleNames.AddRange([
            "Core",
            "CoreUObject",
            "Engine",
            "InputCore",
            "EnhancedInput",
            "AIModule",
            "StateTreeModule",
            "GameplayStateTreeModule",
            "UMG",
            "Slate",
        ]);

        PrivateDependencyModuleNames.AddRange([]);

        PublicIncludePaths.AddRange([
            "PokeSharpSample",
            "PokeSharpSample/Variant_Platforming",
            "PokeSharpSample/Variant_Platforming/Animation",
            "PokeSharpSample/Variant_Combat",
            "PokeSharpSample/Variant_Combat/AI",
            "PokeSharpSample/Variant_Combat/Animation",
            "PokeSharpSample/Variant_Combat/Gameplay",
            "PokeSharpSample/Variant_Combat/Interfaces",
            "PokeSharpSample/Variant_Combat/UI",
            "PokeSharpSample/Variant_SideScrolling",
            "PokeSharpSample/Variant_SideScrolling/AI",
            "PokeSharpSample/Variant_SideScrolling/Gameplay",
            "PokeSharpSample/Variant_SideScrolling/Interfaces",
            "PokeSharpSample/Variant_SideScrolling/UI",
        ]);

        // Uncomment if you are using Slate UI
        // PrivateDependencyModuleNames.AddRange(new string[] { "Slate", "SlateCore" });

        // Uncomment if you are using online features
        // PrivateDependencyModuleNames.Add("OnlineSubsystem");

        // To include OnlineSubsystemSteam, add it to the plugins section in your uproject file with the Enabled attribute set to true
    }
}
