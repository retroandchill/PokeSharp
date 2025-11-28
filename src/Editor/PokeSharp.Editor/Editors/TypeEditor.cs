using PokeSharp.Editor.Core.PokeEdit;
using PokeSharp.Editor.Core.PokeEdit.Schema;
using PokeSharp.Editor.Model;

namespace PokeSharp.Editor.Editors;

public sealed class TypeEditor : IEntityEditor
{
    public TypeDefinition Type { get; } = new()
    {
        Id = "Type",
        Name = "Type",
        Fields = [
            new TextFieldDefinition
            {
                FieldId = "Id",
                Label = "Id",
                AllowEmpty = false
            },
            new TextFieldDefinition
            {
                FieldId = "Name",
                Label = "Name",
                AllowEmpty = false,
                IsLocalizable = true
            },
            new IntFieldDefinition
            {
                FieldId = "IconPosition",
                Label = "Icon Position",
                MinValue = 0
            },
            new BoolFieldDefinition
            {
                FieldId = "IsSpecialType",
                Label = "Is Special Type"
            },
            new BoolFieldDefinition
            {
                FieldId = "IsPseudoType",
                Label = "Is Pseudo Type"
            },
            new ListFieldDefinition
            {
                FieldId = "Weaknesses",
                Label = "Weaknesses",
                ItemField = new ChoiceFieldDefinition
                {
                    FieldId = "Inner",
                    Label = "Inner",
                    Options = new DynamicOptionSourceDefinition
                    {
                        SourceId = "Types"
                    }
                }
            },
            new ListFieldDefinition
            {
                FieldId = "Resistances",
                Label = "Resistances",
                ItemField = new ChoiceFieldDefinition
                {
                    FieldId = "Inner",
                    Label = "Inner",
                    Options = new DynamicOptionSourceDefinition
                    {
                        SourceId = "Types"
                    }
                }
            },
            new ListFieldDefinition
            {
                FieldId = "Immunities",
                Label = "Immunities",
                ItemField = new ChoiceFieldDefinition
                {
                    FieldId = "Inner",
                    Label = "Inner",
                    Options = new DynamicOptionSourceDefinition
                    {
                        SourceId = "Types"
                    }
                }
            },
            new ListFieldDefinition
            {
                FieldId = "Flags",
                Label = "Flags",
                ItemField = new ChoiceFieldDefinition
                {
                    FieldId = "Inner",
                    Label = "Inner",
                    Options = new DynamicOptionSourceDefinition
                    {
                        SourceId = "Types"
                    }
                }
            }
        ]
    };
    
    public Type ClrType => typeof(EditablePokemonType);
}