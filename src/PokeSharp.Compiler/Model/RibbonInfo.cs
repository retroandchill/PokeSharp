using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Data;

namespace PokeSharp.Compiler.Model;

[PbsData("ribbons")]
public class RibbonInfo
{
    [PbsSectionName]
    public required Name Id { get; init; }

    public Text Name { get; set; } = TextConstants.Unnamed;

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int IconPosition { get; set; }

    [PbsType(PbsFieldType.UnformattedText)]
    public Text Description { get; set; } = TextConstants.ThreeQuestions;

    public List<string> Flags { get; set; } = [];
}
