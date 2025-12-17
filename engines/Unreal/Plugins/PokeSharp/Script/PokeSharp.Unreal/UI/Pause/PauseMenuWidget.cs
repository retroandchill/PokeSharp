using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.CommonUI;
using UnrealSharp.Core.Attributes;
using UnrealSharp.PokeSharp;
using UnrealSharp.UMG;

namespace PokeSharp.Unreal.UI.Pause;

[UClass(ClassFlags.Abstract)]
public partial class UPauseMenuWidget : UCommonActivatableWidget
{
    [UProperty]
    [BindWidget]
    private partial UCommandWidget CommandWidget { get; }

    public IList<FCommandData> Commands
    {
        get => CommandWidget.Commands;
        set => CommandWidget.Commands = value;
    }

    public override UWidget? BP_GetDesiredFocusTarget()
    {
        return CommandWidget.IsActive ? CommandWidget : null;
    }

    public Task<TOptional<int>> SelectOptionAsync(CancellationToken cancellationToken = default)
    {
        return CommandWidget.SelectOptionAsync(cancellationToken);
    }
}
