#if POKESHARP_GENERATOR
using RhoMicro.CodeAnalysis;
#endif

// ReSharper disable once CheckNamespace
namespace PokeSharp.Core.Data;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
#if POKESHARP_GENERATOR
[IncludeFile]
#endif
internal class GameDataEntityAttribute : Attribute
{
    public string? DataPath { get; init; }
    
    public bool IsOptional { get; init; } = false;
}
