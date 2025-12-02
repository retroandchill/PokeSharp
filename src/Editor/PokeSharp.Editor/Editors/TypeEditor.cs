using System.Text.Json;
using Injectio.Attributes;
using Microsoft.Extensions.Options;
using PokeSharp.Core.Strings;
using PokeSharp.Data.Pbs;
using PokeSharp.Editor.Core;
using PokeSharp.Editor.Core.PokeEdit;
using PokeSharp.Editor.Core.PokeEdit.Properties;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Editors;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
[EditableType<PokemonType>]
public sealed class TypeEditor(IOptions<JsonSerializerOptions> options) : EntityEditor<PokemonType>(options)
{
    private static readonly Name TypeId = "Type";
    private static readonly Text TypeName = Text.Localized("EntityEditor", "Type", "Type");

    public override Name Id => TypeId;
    public override Text Name => TypeName;
    public override int Order => 0;

    public override IEditableType<PokemonType> Properties => PokemonTypeEditRegistry.Properties;

    public override TypeDefinition Type { get; } =
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
}
