#if POKESHARP_GENERATOR
using RhoMicro.CodeAnalysis;
#endif

namespace PokeSharp.SourceGenerator.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
#if POKESHARP_GENERATOR
[IncludeFile]
#endif
public class GameDataEntityAttribute : Attribute
{
    public string? DataPath { get; init; }
}