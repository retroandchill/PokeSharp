namespace PokeSharp.Editor.Core.PokeEdit.Schema;

public interface IEditableEntity<T>
    where T : class, IEditableEntity<T>
{
    static abstract IEditableType Type { get; }
}
