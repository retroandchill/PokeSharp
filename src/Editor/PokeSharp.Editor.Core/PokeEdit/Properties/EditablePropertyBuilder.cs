using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public interface IEditablePropertyBuilder<TOwner, TValue>
    where TOwner : notnull
{
    IEditablePropertyBuilder<TOwner, TValue> DisplayName(Text name);

    IEditablePropertyBuilder<TOwner, TValue> DefaultValue(TValue defaultValue);

    void Ignore();
}
