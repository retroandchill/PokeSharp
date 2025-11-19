namespace PokeSharp.Compiler.Core;

public record PbsCompilerSettings
{
    public string PbsFileBasePath { get; init; } = "PBS";
    
    public bool AlwaysCompile { get; init; } = false;
}