using System.ComponentModel.DataAnnotations;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Compilers;

public sealed class TypeCompiler : PbsCompiler<PokemonType, PokemonTypeInfo>
{
    public override int Order => 1;

    protected override PokemonType ConvertToEntity(PokemonTypeInfo model) => model.ToGameData();
    protected override PokemonTypeInfo ConvertToModel(PokemonType entity) => entity.ToDto();

    protected override PokemonTypeInfo ValidateCompiledModel(PokemonTypeInfo model)
    {
        // Remove duplicate weaknesses/resistances/immunities
        return model with
        {
            Weaknesses = [..model.Weaknesses.Distinct()],
            Resistances = [..model.Resistances.Distinct()],
            Immunities = [..model.Immunities.Distinct()]
        };
    }

    protected override void ValidateAllCompiledEntities(IReadOnlyList<PokemonType> entities)
    {
        var typeIds = entities.Select(x => x.Id).ToHashSet();
        var exceptions = new List<ValidationException>();
        
        foreach (var type in entities)
        {
            exceptions.AddRange(type.Weaknesses.Where(otherType => !typeIds.Contains(otherType))
                .Select(otherType =>
                    new ValidationException($"'{otherType}' is not a defined type (type {type.Id}, Weaknesses).")));
            exceptions.AddRange(type.Resistances.Where(otherType => !typeIds.Contains(otherType))
                .Select(otherType =>
                    new ValidationException($"'{otherType}' is not a defined type (type {type.Id}, Resistances).")));
            exceptions.AddRange(type.Immunities.Where(otherType => !typeIds.Contains(otherType))
                .Select(otherType =>
                    new ValidationException($"'{otherType}' is not a defined type (type {type.Id}, Immunities).")));
        }
        
        if (exceptions.Count > 0)
        {
            throw new AggregateException("One or more validation errors occurred:", exceptions);
        }
    }
}