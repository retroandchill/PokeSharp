using PokeSharp.Unreal.Editor.Interop;
using UnrealSharp.Engine.Core.Modules;

namespace PokeSharp.Unreal.Editor;

public class FPokeSharpUnrealEditorModule : IModuleInterface
{
    public void StartupModule()
    {
        PokeSharpEditorCallbacksExporter.CallSetOptionSelectionCallbacks(OptionSelectionCallbacks.Create());
        PokeSharpEditorCallbacksExporter.CallSetPokeEditCallbacks(PokeEditCallbacks.Create());
    }

    public void ShutdownModule() { }
}
