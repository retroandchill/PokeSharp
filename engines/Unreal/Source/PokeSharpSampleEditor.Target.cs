// Copyright Epic Games, Inc. All Rights Reserved.

using System.Collections.Generic;
using UnrealBuildTool;

public class PokeSharpSampleEditorTarget : TargetRules
{
    public PokeSharpSampleEditorTarget(TargetInfo target)
        : base(target)
    {
        Type = TargetType.Editor;
        DefaultBuildSettings = BuildSettingsVersion.V6;
        IncludeOrderVersion = EngineIncludeOrderVersion.Unreal5_7;
        ExtraModuleNames.Add("PokeSharpSample");
    }
}
