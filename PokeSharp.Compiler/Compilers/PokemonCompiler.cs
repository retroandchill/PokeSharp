using System.Collections.Immutable;
using System.Runtime.Serialization;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Core.Data;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;
using ZLinq;

namespace PokeSharp.Compiler.Compilers;

public sealed class PokemonCompiler : PbsCompiler<Species, SpeciesInfo>
{
    public override int Order => 8;

    protected override Species ConvertToEntity(SpeciesInfo model) => model.ToGameData();

    protected override SpeciesInfo ConvertToModel(Species entity) => entity.ToDto();

    public override async Task WriteToFile(
        PbsSerializer serializer,
        CancellationToken cancellationToken = default
    )
    {
        await serializer.WritePbsFile(
            FileName,
            Species.AllSpecies.Select(ConvertToModel),
            GetPropertyForPbs
        );
    }

    protected override SpeciesInfo ValidateCompiledModel(
        SpeciesInfo model,
        FileLineData fileLineData
    )
    {
        foreach (
            var evolutionData in model.Evolutions.Select(evolution =>
                Evolution.Get(evolution.Method)
            )
        )
        {
            if (evolutionData.Parameter is null)
                continue;

            throw new PbsParseException(
                $"Evolution method {evolutionData.Name} requires a parameter, but none was given.\n{fileLineData}"
            );
        }

        return model with
        {
            Types = [.. model.Types.Distinct()],
        };
    }

    protected override void ValidateAllCompiledEntities(Span<Species> entities)
    {
        var allSpeciesKeys = entities.AsValueEnumerable().Select(s => s.SpeciesId).ToHashSet();

        var fileLineData = new FileLineData(FileName);
        var newEvolutions = new Dictionary<Name, List<EvolutionInfo>>(entities.Length);
        foreach (var species in entities)
        {
            // Enumerate all offspring and validate them
            fileLineData = fileLineData.WithSection(
                species.SpeciesId,
                nameof(SpeciesInfo.Offspring),
                null
            );
            foreach (
                var offspring in species.Offspring.Where(offspring =>
                    !allSpeciesKeys.Contains(offspring)
                )
            )
            {
                throw new PbsParseException(
                    $"Species '{offspring}' is not defined.\n{fileLineData.LineReport}"
                );
            }

            // Validate all evolutions
            fileLineData = fileLineData.WithSection(
                species.SpeciesId,
                nameof(SpeciesInfo.Evolutions),
                null
            );
            var evolutionList = new List<EvolutionInfo>(species.Evolutions.Length);
            foreach (var evolution in species.Evolutions)
            {
                if (!allSpeciesKeys.Contains(evolution.Species))
                {
                    throw new PbsParseException(
                        $"Species '{evolution.Species}' is not defined.\n{fileLineData.LineReport}"
                    );
                }

                var paramType = Evolution.Get(evolution.EvolutionMethod).Parameter;
                var paramValue = evolution.Parameter?.ToString() ?? string.Empty;
                if (paramType is null)
                {
                    evolutionList.Add(evolution with { Parameter = null });
                }
                else if (paramType == typeof(int))
                {
                    try
                    {
                        evolutionList.Add(
                            evolution with
                            {
                                Parameter = (int)CsvParser.ParseUnsigned(paramValue),
                            }
                        );
                    }
                    catch (SerializationException e)
                    {
                        throw new PbsParseException($"{e}\n{fileLineData.LineReport}", e);
                    }
                }
                else if (
                    paramType.IsEnum
                    || paramType
                        .GetInterfaces()
                        .Any(i =>
                            i.IsGenericType
                            && i.GetGenericTypeDefinition() == typeof(IGameDataEntity<,>)
                        )
                )
                {
                    evolutionList.Add(
                        evolution with
                        {
                            Parameter = CsvParser.ParseEnumField(paramValue, paramType, false),
                        }
                    );
                }
                else
                {
                    evolutionList.Add(evolution);
                }
            }

            newEvolutions.Add(species.SpeciesId, evolutionList);
        }

        // Collect all pre-evolutions and distribute them to all species
        var allEvolutions = newEvolutions
            .SelectMany(s => s.Value, (s, e) => (Species: s.Key, Evolution: e))
            .ToImmutableDictionary(
                x => x.Evolution.Species,
                x => x.Evolution with { Species = x.Species, IsPrevious = true }
            );
        for (var i = 0; i < entities.Length; i++)
        {
            var species = entities[i];
            var speciesId = species.SpeciesId;
            var evolutionList = newEvolutions[speciesId];
            evolutionList.AddRange(allEvolutions[speciesId]);

            entities[i] = species with { Evolutions = [.. evolutionList] };
        }
    }
}
