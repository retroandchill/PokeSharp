using System.Collections.Immutable;

namespace PokeSharp.Editor.SourceGenerator.Model;

public record RequestParameterInfo
{
    public required string Name { get; init; }
    
    public required string ReadType { get; init; }
    
    public required string Generic { get; init; }
    
    public required string SyncPassExpression { get; init; }
    
    public required string AsyncPassExpression { get; init; }

    public bool IsLast { get; init; }
}

public record RequestMethodInfo
{
    public required string Name { get; init; }
    
    public required string? SyncName { get; init; }

    public bool SupportsSync => SyncName is not null;
    
    public required string AsyncName { get; init; }
    
    public required bool IsAsync { get; init; }
    
    public required bool HasCancellationToken { get; init; }
    
    public required string? ResponseWriteType { get; init; }

    public bool HasResponse => ResponseWriteType is not null;
    
    public required ImmutableArray<RequestParameterInfo> Parameters { get; init; }

    public bool HasParameters => Parameters.Length > 0;
};