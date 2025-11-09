using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Data;

namespace PokeSharp.Compiler.Model;

[PbsData("abilities")]
public record AbilityInfo
{
    [PbsSectionName]
    public required Name Id { get; init; }
    
    public Text Name { get; init; } = TextConstants.Unnamed;
    
    [PbsType(PbsFieldType.UnformattedText)]
    public Text Description { get; init; } = TextConstants.ThreeQuestions;
    
    public IReadOnlySet<string> Flags { get; init; } = ImmutableHashSet<string>.Empty;
}