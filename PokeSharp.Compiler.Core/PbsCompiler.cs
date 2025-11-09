using System.Collections;
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
    
    Task WriteToFile(PbsSerializer serializer, CancellationToken cancellationToken = default);
}

public abstract class PbsCompiler<TEntity, TModel> : IPbsCompiler where TEntity : ILoadedGameDataEntity<TEntity>
{
    public abstract int Order { get; }
    private readonly string _fileName;
    private readonly Dictionary<string, PropertyInfo> _propertyMap = new();

    protected PbsCompiler()
    {
        var attribute = typeof(TModel).GetCustomAttribute<PbsDataAttribute>();
        if (attribute is null) throw new InvalidOperationException($"Type {typeof(TModel).FullName} does not have a {nameof(PbsDataAttribute)}");
        _fileName = Path.Join("PBS", $"{attribute.BaseFilename}.txt");
    }

    public async Task Compile(PbsSerializer serializer, CancellationToken cancellationToken = default)
    {
        var entities = await serializer.ReadFromFile<TModel>(_fileName, cancellationToken)
            .Select(ValidateCompiledModel)
            .Select(ConvertToEntity)
            .ToListAsync(cancellationToken: cancellationToken);

        ValidateAllCompiledEntities(entities);
        TEntity.Import(entities);
    }

    public async Task WriteToFile(PbsSerializer serializer, CancellationToken cancellationToken = default)
    {
        await serializer.WritePbsFile(_fileName, TEntity.Entities.Select(ConvertToModel), GetPropertyForPbs);
    }

    protected abstract TEntity ConvertToEntity(TModel model);
    
    protected abstract TModel ConvertToModel(TEntity entity);

    protected virtual TModel ValidateCompiledModel(TModel model)
    {
        return model;
    }

    protected virtual void ValidateAllCompiledEntities(IReadOnlyList<TEntity> entities)
    {
        // No validation by default
    }

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
            case IEnumerable enumerable when TypeUtils.IsEmptyEnumerable(enumerable):
            case false:
                return null;
            default:
                return elementValue;
        }
    }
}