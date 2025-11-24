using Injectio.Attributes;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using PokeSharp.Core.Strings;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Validators;

public interface IEvolutionParameterParser
{
    Type? ParameterType { get; }

    object? Parse(string parameter);
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class NullEvolutionParameterParser : IEvolutionParameterParser
{
    public Type? ParameterType => null;

    public object? Parse(string parameter) => null;
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class IntEvolutionParameterParser : IEvolutionParameterParser
{
    public Type ParameterType => typeof(int);

    public object Parse(string parameter) => CsvParser.ParseInt<int>(parameter);
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class NameEvolutionParameterParser : IEvolutionParameterParser
{
    public Type ParameterType => typeof(Name);

    public object Parse(string parameter) => CsvParser.ParseSymbol(parameter);
}

public abstract class EvolutionParameterParser<TKey, TEntity> : IEvolutionParameterParser
    where TKey : notnull
    where TEntity : IGameDataEntity<TKey, TEntity>
{
    public Type ParameterType => typeof(TEntity);

    public object Parse(string parameter)
    {
        return CsvParser.ParseDataEnum<TEntity, TKey>(parameter);
    }
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class MoveEvolutionParameterParser : EvolutionParameterParser<Name, Move>;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class ItemEvolutionParameterParser : EvolutionParameterParser<Name, Item>;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class TypeEvolutionParameterParser : EvolutionParameterParser<Name, PokemonType>;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class SpeciesEvolutionParameterParser : EvolutionParameterParser<SpeciesForm, Species>;
