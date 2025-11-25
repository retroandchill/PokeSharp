// ReSharper disable once CheckNamespace
namespace UnrealSharp.PokeSharp;

public partial class USelectableWidget
{
    public Task<TOptional<int>> SelectOptionAsync(CancellationToken cancellationToken = default)
    {
        return USelectOptionAsync.SelectOptionAsync(this, cancellationToken);
    }
}
