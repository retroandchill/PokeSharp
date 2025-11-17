using System.Reflection;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Core.Data;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Compiler.Core;

public interface IPbsCompiler
{
    int Order { get; }

    void Compile();

    Task CompileAsync(CancellationToken cancellationToken = default);

    void WriteToFile();

    Task WriteToFileAsync(CancellationToken cancellationToken = default);
}

public abstract class PbsCompilerBase<TModel> : IPbsCompiler
    where TModel : IPbsDataModel<TModel>
{
    public abstract int Order { get; }
    protected string FileName { get; } = Path.Join("PBS", $"{TModel.BasePath}.txt");

    public abstract void Compile();

    public abstract Task CompileAsync(CancellationToken cancellationToken = default);

    public abstract void WriteToFile();

    public abstract Task WriteToFileAsync(CancellationToken cancellationToken = default);
}

public abstract partial class PbsCompiler<TEntity, TModel> : PbsCompilerBase<TModel>
    where TEntity : ILoadedGameDataEntity<TEntity>
    where TModel : IPbsDataModel<TModel>
{
    [CreateSyncVersion]
    public override async Task CompileAsync(CancellationToken cancellationToken = default)
    {
        var entities = await PbsSerializer
            .ReadFromFileAsync<TModel>(FileName, cancellationToken)
            .Select(x =>
            {
                ValidateCompiledModel(x.Model, x.LineData);
                return ConvertToEntity(x.Model);
            })
            .ToArrayAsync(cancellationToken: cancellationToken);

        ValidateAllCompiledEntities(entities);
        await TEntity.ImportAsync(entities, cancellationToken);
    }

    [CreateSyncVersion]
    public override async Task WriteToFileAsync(CancellationToken cancellationToken = default)
    {
        await PbsSerializer.WritePbsFileAsync(FileName, TEntity.Entities.Select(ConvertToModel));
    }

    protected abstract TEntity ConvertToEntity(TModel model);

    protected abstract TModel ConvertToModel(TEntity entity);

    protected virtual void ValidateCompiledModel(TModel model, FileLineData fileLineData)
    {
        // No validation by default
    }

    protected virtual void ValidateAllCompiledEntities(Span<TEntity> entities)
    {
        // No validation by default
    }
}
