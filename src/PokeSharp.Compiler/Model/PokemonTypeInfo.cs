using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Data;

namespace PokeSharp.Compiler.Model;

[PbsData("types")]
public class PokemonTypeInfo
{
    [PbsSectionName]
    public required Name Id { get; init; }

    public Text Name { get; set; } = TextConstants.Unnamed;

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int IconPosition { get; set; }

    public bool IsSpecialType { get; set; }

    public bool IsPseudoType { get; set; }

    public List<Name> Weaknesses { get; set; } = [];

    public List<Name> Resistances { get; set; } = [];

    public List<Name> Immunities { get; set; } = [];

    public List<string> Flags { get; set; } = [];
}
