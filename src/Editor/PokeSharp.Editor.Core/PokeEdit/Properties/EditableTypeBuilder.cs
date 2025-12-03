using System.Linq.Expressions;
using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public interface IEditableTypeBuilder<T>
    where T : notnull
{
    IEditableTypeBuilder<T> DisplayName(Text displayName);

    IEditableTypeBuilder<T> Property<TValue>(
        Expression<Func<T, TValue>> propertyExpression,
        Expression<Action<IEditablePropertyBuilder<T, TValue>>> customize
    );
}
