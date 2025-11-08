using System.Collections.Immutable;
using System.Reflection;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Core.Data;

namespace PokeSharp.Compiler.Core;

public interface IPbsCompiler
{
    int Order { get; }
    
    Task Compile(PbsSerializer serializer, CancellationToken cancellationToken = default);
}

public abstract class PbsCompiler<TEntity, TModel> : IPbsCompiler where TEntity : ILoadedGameDataEntity<TEntity>
{
    public abstract int Order { get; }
    private readonly ImmutableArray<string> _fileNames;

    protected PbsCompiler()
    {
        var attribute = typeof(TModel).GetCustomAttribute<PbsDataAttribute>();
        _fileNames = attribute?.BaseFilenames.Select(x => Path.Join("PBS", $"{x}.txt")).ToImmutableArray() ?? [];
    }

    public async Task Compile(PbsSerializer serializer, CancellationToken cancellationToken = default)
    {
        var entities = await serializer.ReadFromFile<TModel>(_fileNames, cancellationToken)
            .Select(ValidateCompiledModel)
            .Select(Convert)
            .ToListAsync(cancellationToken: cancellationToken);

        ValidateAllCompiledEntities(entities);
        TEntity.Import(entities);
    }
    
    protected abstract TEntity Convert(TModel model);

    protected virtual TModel ValidateCompiledModel(TModel model)
    {
        return model;
    }

    protected virtual void ValidateAllCompiledEntities(IReadOnlyList<TEntity> entities)
    {
        // No validation by default
    }
}