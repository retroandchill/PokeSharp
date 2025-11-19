using System.ComponentModel.DataAnnotations;
using Injectio.Attributes;
using Microsoft.Extensions.Options;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Compiler.Core.Utils;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Core.Utils;
using PokeSharp.Data.Pbs;
using ZLinq;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class TypeCompiler(IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings) : PbsCompiler<PokemonType, PokemonTypeInfo>(pbsCompileSettings)
{
    public override int Order => 3;

    protected override PokemonType ConvertToEntity(PokemonTypeInfo model) => model.ToGameData();

    protected override PokemonTypeInfo ConvertToModel(PokemonType entity) => entity.ToDto();

    protected override void ValidateCompiledModel(PokemonTypeInfo model, FileLineData fileLineData)
    {
        // Remove duplicate weaknesses/resistances/immunities
        model.Weaknesses.DistinctInPlace();
        model.Resistances.DistinctInPlace();
        model.Immunities.DistinctInPlace();
    }

    protected override void ValidateAllCompiledEntities(Span<PokemonType> entities)
    {
        var typeIds = entities.AsValueEnumerable().Select(x => x.Id).ToHashSet();
        var exceptions = new List<ValidationException>();

        foreach (var type in entities)
        {
            exceptions.AddRange(
                type.Weaknesses.Where(otherType => !typeIds.Contains(otherType))
                    .Select(otherType => new ValidationException(
                        $"'{otherType}' is not a defined type (type {type.Id}, Weaknesses)."
                    ))
            );
            exceptions.AddRange(
                type.Resistances.Where(otherType => !typeIds.Contains(otherType))
                    .Select(otherType => new ValidationException(
                        $"'{otherType}' is not a defined type (type {type.Id}, Resistances)."
                    ))
            );
            exceptions.AddRange(
                type.Immunities.Where(otherType => !typeIds.Contains(otherType))
                    .Select(otherType => new ValidationException(
                        $"'{otherType}' is not a defined type (type {type.Id}, Immunities)."
                    ))
            );
        }

        if (exceptions.Count > 0)
        {
            throw new AggregateException("One or more validation errors occurred:", exceptions);
        }
    }
}
