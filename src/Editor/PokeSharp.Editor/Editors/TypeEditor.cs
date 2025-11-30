using Injectio.Attributes;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit;
using PokeSharp.Editor.Core.PokeEdit.Schema;
using PokeSharp.Editor.Model;

namespace PokeSharp.Editor.Editors;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class TypeEditor : IEntityEditor
{
    private static readonly Name TypeId = "Type";
    private static readonly Text TypeName = Text.Localized("EntityEditor", "Type", "Type");

    public Name Id => TypeId;
    public Text Name => TypeName;
    public int Order => 0;

    public TypeDefinition Type { get; } =
        new()
        {
            Id = "Type",
            Name = "Type",
            Fields =
            [
                new TextFieldDefinition
                {
                    FieldId = "Id",
                    Label = "Id",
                    AllowEmpty = false,
                },
                new TextFieldDefinition
                {
                    FieldId = "Name",
                    Label = "Name",
                    AllowEmpty = false,
                    IsLocalizable = true,
                },
                new IntFieldDefinition
                {
                    FieldId = "IconPosition",
                    Label = "Icon Position",
                    MinValue = 0,
                },
                new BoolFieldDefinition { FieldId = "IsSpecialType", Label = "Is Special Type" },
                new BoolFieldDefinition { FieldId = "IsPseudoType", Label = "Is Pseudo Type" },
                new ListFieldDefinition
                {
                    FieldId = "Weaknesses",
                    Label = "Weaknesses",
                    ItemField = new ChoiceFieldDefinition
                    {
                        FieldId = "Inner",
                        Label = "Inner",
                        Options = new DynamicOptionSourceDefinition { SourceId = "Types" },
                    },
                },
                new ListFieldDefinition
                {
                    FieldId = "Resistances",
                    Label = "Resistances",
                    ItemField = new ChoiceFieldDefinition
                    {
                        FieldId = "Inner",
                        Label = "Inner",
                        Options = new DynamicOptionSourceDefinition { SourceId = "Types" },
                    },
                },
                new ListFieldDefinition
                {
                    FieldId = "Immunities",
                    Label = "Immunities",
                    ItemField = new ChoiceFieldDefinition
                    {
                        FieldId = "Inner",
                        Label = "Inner",
                        Options = new DynamicOptionSourceDefinition { SourceId = "Types" },
                    },
                },
                new ListFieldDefinition
                {
                    FieldId = "Flags",
                    Label = "Flags",
                    ItemField = new ChoiceFieldDefinition
                    {
                        FieldId = "Inner",
                        Label = "Inner",
                        Options = new DynamicOptionSourceDefinition { SourceId = "Types" },
                    },
                },
            ],
        };

    public Type ClrType => typeof(EditablePokemonType);
}
