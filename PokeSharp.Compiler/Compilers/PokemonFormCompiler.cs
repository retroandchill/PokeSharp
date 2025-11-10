using System.Collections;
using System.Reflection;
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

namespace PokeSharp.Compiler.Compilers;

public class PokemonFormCompiler : PbsCompiler<Species, SpeciesFormInfo>
{
    public override int Order => 9;

    private static readonly Dictionary<string, PropertyInfo> PropertyMap = new();

    static PokemonFormCompiler()
    {
        foreach (var property in typeof(SpeciesFormInfo).GetProperties())
        {
            var attribute = property.GetCustomAttribute<PbsFieldBaseAttribute>();
            var name = attribute?.Name ?? property.Name;
            PropertyMap.Add(name, property);
        }
    }

    public override async Task Compile(
        PbsSerializer serializer,
        CancellationToken cancellationToken = default
    )
    {
        var entities = await serializer
            .ReadFromFile(
                FileName,
                name => Species.Get(name).ToSpeciesFormInfo(),
                cancellationToken
            )
            .Select(x => ValidateCompiledModel(x.Model, x.LineData))
            .Select(ConvertToEntity)
            .ToArrayAsync(cancellationToken: cancellationToken);

        Species.Import(ValidateAllCompiledForms(entities));
    }

    public override async Task WriteToFile(
        PbsSerializer serializer,
        CancellationToken cancellationToken = default
    )
    {
        await serializer.WritePbsFile(
            FileName,
            Species.Entities.Where(s => s.Form > 0).Select(ConvertToModel),
            GetPropertyForPbs
        );
    }

    protected override Species ConvertToEntity(SpeciesFormInfo model)
    {
        var baseForm = Species.GetSpeciesForm(model.Id.Species, 0);
        return model.ToGameData(
            baseForm.Name,
            baseForm.GrowthRate,
            baseForm.GenderRatio,
            baseForm.Incense
        );
    }

    protected override SpeciesFormInfo ConvertToModel(Species entity)
    {
        throw new NotImplementedException();
    }

    protected override SpeciesFormInfo ValidateCompiledModel(
        SpeciesFormInfo model,
        FileLineData fileLineData
    )
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

        return model with
        {
            Types = [.. model.Types.Distinct()],
        };
    }

    private List<Species> ValidateAllCompiledForms(Species[] entities)
    {
        var allSpecies = Species.AllSpecies.Concat(entities).ToList();

        var fileLineData = new FileLineData(FileName);
        var newEvolutions = new Dictionary<SpeciesForm, List<EvolutionInfo>>(entities.Length);
        foreach (var species in allSpecies)
        {
            // Validate all evolutions
            fileLineData = fileLineData.WithSection(
                species.SpeciesId,
                nameof(SpeciesInfo.Evolutions),
                null
            );
            var evolutionList = new List<EvolutionInfo>(species.Evolutions.Length);
            foreach (var evolution in species.Evolutions)
            {
                var paramType = Evolution.TryGet(evolution.EvolutionMethod, out var evo)
                    ? evo.Parameter
                    : null;
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
                    allEvolutions.Add(
                        evo.Species,
                        evo with
                        {
                            Species = species.SpeciesId,
                            IsPrevious = true,
                        }
                    );
                }

                var formKey = new SpeciesForm(evo.Species, species.Form);
                if (species.Form > 0 && !allEvolutions.ContainsKey(formKey))
                {
                    allEvolutions.Add(
                        formKey,
                        evo with
                        {
                            Species = species.SpeciesId,
                            IsPrevious = true,
                        }
                    );
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

    protected override object? GetPropertyForPbs(SpeciesFormInfo model, string key)
    {
        var original = base.GetPropertyForPbs(model, key);
        if (original is null)
            return null;

        if (
            key is nameof(SpeciesFormInfo.Height) or nameof(SpeciesFormInfo.Weight)
            && original is decimal asDecimal
        )
        {
            original = asDecimal.ToString("0.0");
        }

        var baseForm = Species.GetSpeciesForm(model.Id.Species, 0).ToSpeciesFormInfo();

        if (
            key
                is nameof(SpeciesFormInfo.WildItemCommon)
                    or nameof(SpeciesFormInfo.WildItemUncommon)
                    or nameof(SpeciesFormInfo.WildItemRare)
            && (
                !baseForm.WildItemCommon.SequenceEqual(model.WildItemCommon)
                || !baseForm.WildItemUncommon.SequenceEqual(model.WildItemUncommon)
                || !baseForm.WildItemRare.SequenceEqual(model.WildItemRare)
            )
        )
        {
            return original;
        }

        var property = PropertyMap[key];
        var baseFormValue = property.GetValue(baseForm);

        return !CompareEqual(original, baseFormValue) ? original : null;
    }

    private static bool CompareEqual(object? object1, object? object2)
    {
        if (Equals(object1, object2))
            return true;

        if (object1 is IEnumerable collection1 && object2 is IEnumerable collection2)
        {
            return collection1
                .Cast<object?>()
                .Zip(collection2.Cast<object?>())
                .All(x => CompareEqual(x.First, x.Second));
        }

        return false;
    }
}
