using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Data;

namespace PokeSharp.Compiler.Model;

[PbsData("ribbons")]
public record RibbonInfo
{
    [PbsSectionName]
    public required Name Id { get; init; }

    public Text Name { get; init; } = TextConstants.Unnamed;

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int IconPosition { get; init; }

    [PbsType(PbsFieldType.UnformattedText)]
    public Text Description { get; init; } = TextConstants.ThreeQuestions;

    public ImmutableArray<string> Flags { get; init; } = [];
}
