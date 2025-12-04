using System.Linq.Expressions;
using System.Text.Json;
using PokeSharp.Core.Collections.Immutable;
using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public sealed class EditorModelBuilder(JsonSerializerOptions jsonSerializerOptions)
{
    private readonly ImmutableOrderedDictionary<Name, IEntityEditor>.Builder _builder =
        ImmutableOrderedDictionary.CreateBuilder<Name, IEntityEditor>();

    public JsonSerializerOptions JsonSerializerOptions { get; } = jsonSerializerOptions;

    public EditorModelBuilder For<T>(Expression<Action<IEditableTypeBuilder<T>>> configure)
        where T : notnull
    {
        return this;
    }

    public EditorModelBuilder Add(IEntityEditor editor)
    {
        _builder.Add(editor.Id, editor);
        return this;
    }

    public ImmutableOrderedDictionary<Name, IEntityEditor> Build() => _builder.ToImmutable();
}
