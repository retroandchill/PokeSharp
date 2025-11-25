using UnrealSharp;
using UnrealSharp.Attributes;
using UnrealSharp.Attributes.MetaTags;
using UnrealSharp.CommonUI;
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

    protected override UWidget? BP_GetDesiredFocusTarget_Implementation()
    {
        return CommandWidget.IsActive ? CommandWidget : null;
    }

    public Task<TOptional<int>> SelectOptionAsync(CancellationToken cancellationToken = default)
    {
        return CommandWidget.SelectOptionAsync(cancellationToken);
    }
}
