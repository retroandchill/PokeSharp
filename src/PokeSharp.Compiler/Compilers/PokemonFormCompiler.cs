using System.Collections.Immutable;
using Injectio.Attributes;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Compiler.Validators;
using PokeSharp.Core.Utils;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public sealed partial class PokemonFormCompiler(IEnumerable<IEvolutionParameterParser> evolutionParameterParsers)
    : PbsCompilerBase<SpeciesFormInfo>
{
    public override int Order => 9;

    private readonly ImmutableArray<IEvolutionParameterParser> _evolutionParsers = [.. evolutionParameterParsers];

    [CreateSyncVersion]
    public override async Task CompileAsync(CancellationToken cancellationToken = default)
    {
        var entities = await PbsSerializer
            .ReadFromFileAsync<SpeciesFormInfo>(
                FileName,
                name =>
                {
                    var formInfo = Species.Get(name.Split(",")[0]).ToSpeciesFormInfo();
                    formInfo.WildItemCommon = [];
                    formInfo.WildItemUncommon = [];
                    formInfo.WildItemRare = [];
                    return formInfo;
                },
                cancellationToken
            )
            .Select(x =>
            {
                ValidateCompiledModel(x.Model, x.LineData);
                return ConvertToEntity(x.Model);
            })
            .ToArrayAsync(cancellationToken: cancellationToken);

        await Species.ImportAsync(ValidateAllCompiledForms(entities), cancellationToken);
    }

    [CreateSyncVersion]
    public override async Task WriteToFileAsync(CancellationToken cancellationToken = default)
    {
        await PbsSerializer.WritePbsFileAsync(FileName, Species.Entities.Where(s => s.Form > 0).Select(ConvertToModel));
    }

    private static Species ConvertToEntity(SpeciesFormInfo model)
    {
        var baseForm = Species.Get(model.Id.Species, 0);
        return model.ToGameData(baseForm.Name, baseForm.GrowthRate, baseForm.GenderRatio, baseForm.Incense);
    }

    private static SpeciesFormInfo ConvertToModel(Species entity)
    {
        var model = entity.ToSpeciesFormInfo();
        var baseForm = Species.Get(model.Id.Species, 0).ToSpeciesFormInfo();
        if (
            !SequenceEqual(baseForm.WildItemCommon, model.WildItemCommon)
            || !SequenceEqual(baseForm.WildItemUncommon, model.WildItemUncommon)
            || !SequenceEqual(baseForm.WildItemRare, model.WildItemRare)
        )
            return model;

        model.WildItemCommon = [];
        model.WildItemUncommon = [];
        model.WildItemRare = [];

        return model;
    }

    private static void ValidateCompiledModel(SpeciesFormInfo model, FileLineData fileLineData)
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

        var baseData = Species.Get(model.Id.Species, 0);

        if (model.WildItemCommon.Count == 0 && model.WildItemUncommon.Count == 0 && model.WildItemRare.Count == 0)
        {
            model.WildItemCommon = baseData.WildItemCommon.ToList();
            model.WildItemUncommon = baseData.WildItemUncommon.ToList();
            model.WildItemRare = baseData.WildItemRare.ToList();
        }

        model.Types.DistinctInPlace();
    }

    private List<Species> ValidateAllCompiledForms(Species[] entities)
    {
        var allSpecies = Species.AllSpecies.Concat(entities).ToList();

        var fileLineData = new FileLineData(FileName);
        var newEvolutions = new Dictionary<SpeciesForm, List<EvolutionInfo>>(entities.Length);
        foreach (var species in allSpecies)
        {
            // Validate all evolutions
            fileLineData = fileLineData.WithSection(species.SpeciesId, nameof(SpeciesInfo.Evolutions), null);
            var evolutionList = new List<EvolutionInfo>(species.Evolutions.Length);
            foreach (var evolution in species.Evolutions)
            {
                var paramType = Evolution.TryGet(evolution.EvolutionMethod, out var evo) ? evo.Parameter : null;
                var paramValue = evolution.Parameter?.ToString() ?? string.Empty;
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

            newEvolutions.Add(species.Id, evolutionList);
        }

        var allEvolutions = new Dictionary<SpeciesForm, EvolutionInfo>();
        foreach (var species in allSpecies)
        {
            if (!newEvolutions.TryGetValue(species.Id, out var evolutions))
            {
                evolutions = [];
                newEvolutions.Add(species.Id, evolutions);
            }

            foreach (var evo in evolutions.Where(evo => !evo.IsPrevious))
            {
                if (!allEvolutions.ContainsKey(evo.Species))
                {
                    allEvolutions.Add(evo.Species, evo with { Species = species.SpeciesId, IsPrevious = true });
                }

                var formKey = new SpeciesForm(evo.Species, species.Form);
                if (species.Form > 0 && !allEvolutions.ContainsKey(formKey))
                {
                    allEvolutions.Add(formKey, evo with { Species = species.SpeciesId, IsPrevious = true });
                }
            }
        }

        foreach (var species in allSpecies)
        {
            var formKey = new SpeciesForm(species.SpeciesId, species.BaseForm);
            var previousData = allEvolutions.TryGetValue(formKey, out var e)
                ? e
                : allEvolutions.GetValueOrDefault(species.SpeciesId);
            if (previousData is null)
                continue;

            var newEvolutionsList = newEvolutions[species.Id];
            newEvolutionsList.RemoveAll(x => x.IsPrevious);
            newEvolutionsList.Add(previousData);

            var previousSpecies = newEvolutions[previousData.Species];
            if (previousSpecies.All(x => x.IsPrevious || x.Species != species.SpeciesId))
            {
                previousSpecies.Add(new EvolutionInfo(species.SpeciesId, Name.None));
            }
        }

        return allSpecies.Select(x => x with { Evolutions = [.. newEvolutions[x.Id]] }).ToList();
    }

    private static bool SequenceEqual<T>(IEnumerable<T>? enumerable1, IEnumerable<T>? enumerable2)
    {
        if (ReferenceEquals(enumerable1, enumerable2))
            return true;
        if (enumerable1 is null || enumerable2 is null)
            return false;
        return enumerable1.SequenceEqual(enumerable2);
    }
}
