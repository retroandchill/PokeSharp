namespace PokeSharp.Compiler.Core.Serialization;

public interface IPbsDataModel<TModel>
    where TModel : IPbsDataModel<TModel>
{
    static abstract string BasePath { get; }

    static abstract bool IsOptional { get; }

    static abstract TModel ParsePbsData(PbsSection section, Func<string, TModel>? factory = null);

    IEnumerable<string> WritePbsData();
}
