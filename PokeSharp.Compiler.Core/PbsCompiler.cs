using System.Collections;
using System.Reflection;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Compiler.Core.Utils;
using PokeSharp.Core.Data;

namespace PokeSharp.Compiler.Core;

public interface IPbsCompiler
{
    int Order { get; }

    Task Compile(PbsSerializer serializer, CancellationToken cancellationToken = default);

    Task WriteToFile(PbsSerializer serializer, CancellationToken cancellationToken = default);
}

public abstract class PbsCompilerBase<TModel> : IPbsCompiler
{
    public abstract int Order { get; }
    protected string FileName { get; }
    private readonly Dictionary<string, PropertyInfo> _propertyMap = new();

    protected PbsCompilerBase()
    {
        var attribute = typeof(TModel).GetCustomAttribute<PbsDataAttribute>();
        if (attribute is null)
            throw new InvalidOperationException(
                $"Type {typeof(TModel).FullName} does not have a {nameof(PbsDataAttribute)}"
            );
        FileName = Path.Join("PBS", $"{attribute.BaseFilename}.txt");
    }

    public abstract Task Compile(
        PbsSerializer serializer,
        CancellationToken cancellationToken = default
    );

    public abstract Task WriteToFile(
        PbsSerializer serializer,
        CancellationToken cancellationToken = default
    );

    protected virtual object? GetPropertyForPbs(TModel model, string key)
    {
        if (!_propertyMap.TryGetValue(key, out var property))
        {
            property = typeof(TModel).GetProperty(key);
            ArgumentNullException.ThrowIfNull(property);
            _propertyMap.Add(key, property);
        }

        var elementValue = property.GetValue(model);
        switch (elementValue)
        {
            case IEnumerable enumerable when CollectionUtils.IsEmptyEnumerable(enumerable):
            case false:
                return null;
            default:
                return elementValue;
        }
    }
}

public abstract class PbsCompiler<TEntity, TModel> : PbsCompilerBase<TModel>
    where TEntity : ILoadedGameDataEntity<TEntity>
{
    public override async Task Compile(
        PbsSerializer serializer,
        CancellationToken cancellationToken = default
    )
    {
        var entities = await serializer
            .ReadFromFile<TModel>(FileName, cancellationToken)
            .Select(x =>
            {
                ValidateCompiledModel(x.Model, x.LineData);
                return ConvertToEntity(x.Model);
            })
            .ToArrayAsync(cancellationToken: cancellationToken);

        ValidateAllCompiledEntities(entities);
        TEntity.Import(entities);
    }

    public override async Task WriteToFile(
        PbsSerializer serializer,
        CancellationToken cancellationToken = default
    )
    {
        await serializer.WritePbsFile(
            FileName,
            TEntity.Entities.Select(ConvertToModel),
            GetPropertyForPbs
        );
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
