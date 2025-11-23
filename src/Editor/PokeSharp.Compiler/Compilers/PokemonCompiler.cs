using System.Collections.Immutable;
using Injectio.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Core.Logging;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Compiler.Validators;
using PokeSharp.Core;
using PokeSharp.Core.Utils;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;
using ZLinq;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed class PokemonCompiler(
    ILogger<PokemonCompiler> logger,
    IEnumerable<IEvolutionParameterParser> evolutionParameterParsers,
    IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings,
    PbsSerializer serializer
) : PbsCompiler<Species, SpeciesInfo>(logger, pbsCompileSettings, serializer)
{
    public override int Order => 8;

    private readonly ImmutableArray<IEvolutionParameterParser> _evolutionParsers = [.. evolutionParameterParsers];

    public override async Task WriteToFileAsync(CancellationToken cancellationToken = default)
    {
        Logger.LogWritingPbsFile(Path.GetFileName(FileName));
        await Serializer.WritePbsFileAsync(FileName, Species.AllSpecies.Select(ConvertToModel));
    }

    protected override Species ConvertToEntity(SpeciesInfo model) => model.ToGameData();

    protected override SpeciesInfo ConvertToModel(Species entity) => entity.ToDto();

    protected override void ValidateCompiledModel(SpeciesInfo model, FileLineData fileLineData)
    {
        foreach (var evolution in model.Evolutions.Where(evo => evo.Method.IsValid))
        {
            var evolutionData = Evolution.Get(evolution.Method);
            if (evolution.Parameter is not null || evolutionData.Parameter is null)
                continue;

            throw new PbsParseException(
                $"Evolution method {evolutionData.Name} requires a parameter, but none was given.\n{fileLineData}"
            );
        }

        model.Types.DistinctInPlace();
    }

    protected override void ValidateAllCompiledEntities(Span<Species> entities)
    {
        var allSpeciesKeys = entities.AsValueEnumerable().Select(s => s.SpeciesId).ToHashSet();

        var fileLineData = new FileLineData(FileName);
        var newEvolutions = new Dictionary<Name, List<EvolutionInfo>>(entities.Length);
        foreach (var species in entities)
        {
            // Enumerate all offspring and validate them
            fileLineData = fileLineData.WithSection(species.SpeciesId, nameof(SpeciesInfo.Offspring), null);
            foreach (var offspring in species.Offspring.Where(offspring => !allSpeciesKeys.Contains(offspring)))
            {
                throw new PbsParseException($"Species '{offspring}' is not defined.\n{fileLineData.LineReport}");
            }

            // Validate all evolutions
            fileLineData = fileLineData.WithSection(species.SpeciesId, nameof(SpeciesInfo.Evolutions), null);
            var evolutionList = new List<EvolutionInfo>(species.Evolutions.Length);
            foreach (var evolution in species.Evolutions)
            {
                if (!allSpeciesKeys.Contains(evolution.Species))
                {
                    throw new PbsParseException(
                        $"Species '{evolution.Species}' is not defined.\n{fileLineData.LineReport}"
                    );
                }

                var paramType = Evolution.TryGet(evolution.EvolutionMethod, out var evo) ? evo.Parameter : null;
                var paramValue = evolution.Parameter?.ToString() ?? string.Empty;

                if (paramType == typeof(Species))
                {
                    var asName = new Name(paramValue);
                    if (!allSpeciesKeys.Contains(asName))
                        throw new PbsParseException($"Species '{asName}' is not defined.\n{fileLineData.LineReport}");

                    evolutionList.Add(evolution with { Parameter = asName });
                }
                else
                {
                    var parameterParser = _evolutionParsers.SingleOrDefault(p => p.ParameterType == paramType);
                    if (parameterParser is not null)
                    {
                        evolutionList.Add(evolution with { Parameter = parameterParser.Parse(paramValue) });
                    }
                    else
                    {
                        evolutionList.Add(evolution);
                    }
                }
            }

            newEvolutions.Add(species.SpeciesId, evolutionList);
        }

        // Collect all pre-evolutions and distribute them to all species
        var allEvolutions = new Dictionary<Name, EvolutionInfo>();
        foreach (var species in entities)
        {
            if (!newEvolutions.TryGetValue(species.SpeciesId, out var evolutions))
            {
                evolutions = [];
                newEvolutions.Add(species.SpeciesId, evolutions);
            }

            foreach (var evo in evolutions.Where(evo => !allEvolutions.ContainsKey(evo.Species)))
            {
                allEvolutions.Add(evo.Species, evo with { Species = species.SpeciesId, IsPrevious = true });
            }
        }
        for (var i = 0; i < entities.Length; i++)
        {
            var species = entities[i];
            var speciesId = species.SpeciesId;
            var evolutionList = newEvolutions[speciesId];
            if (allEvolutions.TryGetValue(speciesId, out var previousEvolutions))
            {
                evolutionList.Add(previousEvolutions);
            }

            entities[i] = species with { Evolutions = [.. evolutionList] };
        }
    }
}
