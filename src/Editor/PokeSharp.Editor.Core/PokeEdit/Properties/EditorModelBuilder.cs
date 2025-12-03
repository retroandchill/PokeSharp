using System.Linq.Expressions;
using PokeSharp.Core.Collections.Immutable;
using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public sealed class EditorModelBuilder
{
    private readonly ImmutableOrderedDictionary<Name, IEntityEditor>.Builder _builder =
        ImmutableOrderedDictionary.CreateBuilder<Name, IEntityEditor>();

    public EditorModelBuilder For<T>(Expression<Action<IEditableTypeBuilder<T>>> configure)
        where T : notnull
    {
        return this;
    }

    public ImmutableOrderedDictionary<Name, IEntityEditor> Build() => _builder.ToImmutable();
}
