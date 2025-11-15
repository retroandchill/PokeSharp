using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Data;

namespace PokeSharp.Compiler.Model;

[PbsData("abilities")]
public class AbilityInfo
{
    [PbsSectionName]
    public required Name Id { get; init; }

    public Text Name { get; set; } = TextConstants.Unnamed;

    [PbsType(PbsFieldType.UnformattedText)]
    public Text Description { get; set; } = TextConstants.ThreeQuestions;

    public List<string> Flags { get; set; } = [];
}
