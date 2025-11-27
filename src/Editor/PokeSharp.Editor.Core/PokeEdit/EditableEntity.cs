namespace PokeSharp.Editor.Core.PokeEdit;

public interface IEditableEntity<TSelf>
    where TSelf : class, IEditableEntity<TSelf>
{
    static abstract IEditableType Type { get; }
}
