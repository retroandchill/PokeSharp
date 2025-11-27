using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit;

public interface IProperty
{
    public Name Name { get; }
    public PropertyKind Kind { get; }
    public Type Type { get; }
    public IEditableType Owner { get; }
    public bool IsReadOnly { get; }

    public object? GetValue(object target);
    public void SetValue(object target, object? value);
}
