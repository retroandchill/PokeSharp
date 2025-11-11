using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using System.Text.RegularExpressions;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Compiler.Core.Utils;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;
using PokeSharp.Game;

namespace PokeSharp.Compiler.Compilers;

public class TrainerCompiler : PbsCompilerBase<EnemyTrainerInfo>
{
    public override int Order => 15;

    private static readonly Dictionary<string, PropertyInfo> _pokemonPropertyMap = new();

    private static readonly SchemaEntry PokemonSchemaEntry = new(
        typeof(EnemyTrainerInfo).GetProperty(nameof(EnemyTrainerInfo.Pokemon))!,
        [
            new SchemaTypeData(PbsFieldType.Enumerable, false, typeof(Species)),
            new SchemaTypeData(PbsFieldType.PositiveInteger),
        ]
    );

    public override async Task Compile(
        PbsSerializer serializer,
        CancellationToken cancellationToken = default
    )
    {
        var schema = serializer.GetSchema(typeof(EnemyTrainerInfo)).ToDictionary();
        schema.Add(PokemonSchemaEntry.PropertyName, PokemonSchemaEntry);
        var subschema = serializer.GetSchema(typeof(TrainerPokemonInfo));

        EnemyTrainerInfo? currentTrainer = null;
        TrainerPokemonInfo? currentPokemon = null;
        string? sectionName = null;
        string? sectionLine = null;

        var fileLineData = new FileLineData(FileName);
        var result = new List<EnemyTrainer>();
        await foreach (
            var (line, lineNumber) in serializer.ParsePreppedLines(FileName, cancellationToken)
        )
        {
            var matchSectionHeader = PbsSerializer.SectionHeader.Match(line);
            if (matchSectionHeader.Success)
            {
                sectionName = matchSectionHeader.Groups[1].Value;
                sectionLine = line;

                if (currentTrainer is not null)
                {
                    ValidateCompiledTrainer(currentTrainer, fileLineData);
                    result.Add(currentTrainer.ToGameData());
                }

                fileLineData = fileLineData.WithSection(sectionName, null, sectionLine);

                try
                {
                    var sectionNameSchema = schema["SectionName"];
                    var record = CsvParser.GetCsvRecord(sectionName, sectionNameSchema);
                    var converted = (TrainerKey)
                        ConversionUtils.ConvertTypeIfNecessary(
                            sectionName,
                            record,
                            sectionNameSchema.Property.PropertyType,
                            sectionNameSchema.Property,
                            serializer.Converters
                        )!;
                    currentTrainer = new EnemyTrainerInfo { Id = converted };
                }
                catch (Exception e)
                {
                    throw PbsParseException.Create(e, fileLineData);
                }
            }

            var matchKey = PbsSerializer.KeyValuePair.Match(line);
            if (!matchKey.Success)
                continue;

            if (currentTrainer is null)
            {
                throw new PbsParseException(
                    $"Expected a section at the beginning of the file.\n{fileLineData.LineReport}"
                );
            }

            var key = matchKey.Groups[1].Value;
            var value = matchKey.Groups[2].Value;

            if (schema.TryGetValue(key, out var entry))
            {
                var propertyValue = CsvParser.GetCsvRecord(value, entry);
                ArgumentNullException.ThrowIfNull(propertyValue);
                if (key == nameof(EnemyTrainerInfo.Pokemon))
                {
                    var asList = (IList)propertyValue;
                    currentPokemon = new TrainerPokemonInfo
                    {
                        Species = ((SpeciesForm)asList[0]!).Species,
                        Level = (int)(ulong)asList[1]!,
                    };
                    currentTrainer.Pokemon.Add(currentPokemon);
                }
                else
                {
                    ConversionUtils.SetValueToProperty(
                        key,
                        currentTrainer,
                        entry.Property,
                        propertyValue,
                        serializer.Converters
                    );
                }
            }
            else if (subschema.TryGetValue(key, out var subentry))
            {
                if (currentPokemon is null)
                {
                    throw new PbsParseException(
                        $"Pokémon hasn't been defined yet!\n{fileLineData.LineReport}"
                    );
                }

                ConversionUtils.SetValueToProperty(
                    key,
                    currentPokemon,
                    subentry.Property,
                    CsvParser.GetCsvRecord(value, subentry),
                    serializer.Converters
                );
            }
        }

        if (currentTrainer is not null)
        {
            fileLineData = fileLineData.WithSection(sectionName, null, sectionLine);
            ValidateCompiledTrainer(currentTrainer, fileLineData);
            result.Add(currentTrainer.ToGameData());
        }

        EnemyTrainer.Import(result);
    }

    private static void ValidateCompiledTrainer(EnemyTrainerInfo trainer, FileLineData fileLineData)
    {
        if (trainer.Pokemon.Count == 0)
        {
            throw new PbsParseException(
                $"Trainer with ID '{trainer.Id}' has no Pokémon.\n{fileLineData.LineReport}"
            );
        }

        var maxLevel = GrowthRate.MaxLevel;
        var mainStats = Stat.AllMain.Where(s => s.PbsOrder >= 0).ToImmutableArray();
        Span<int> newIVs = stackalloc int[mainStats.Length];
        Span<int> newEVs = stackalloc int[mainStats.Length];
        foreach (var pokemon in trainer.Pokemon)
        {
            if (pokemon.Level > maxLevel)
            {
                throw new PbsParseException(
                    $"Invalid Pokémon level {pokemon.Level} (must be 1-{maxLevel}).\n{fileLineData.LineReport}"
                );
            }

            if (
                pokemon.Name.HasValue
                && pokemon.Name.Value.AsReadOnlySpan().Length > Pokemon.NameSizeLimit
            )
            {
                throw new PbsParseException(
                    $"Invalid Pokémon nickname: {pokemon.Name.Value} (must be 1-{Pokemon.NameSizeLimit} characters).\n{fileLineData.LineReport}"
                );
            }

            pokemon.Moves?.DistinctInPlace();

            if (pokemon.IV is not null)
            {
                foreach (var stat in mainStats)
                {
                    newIVs[stat.PbsOrder] =
                        pokemon.IV.Count > stat.PbsOrder
                            ? pokemon.IV[stat.PbsOrder]
                            : pokemon.IV[0];
                    if (newIVs[stat.PbsOrder] > Pokemon.IVStatLimit)
                    {
                        throw new PbsParseException(
                            $"Invalid IV: {newIVs[stat.PbsOrder]} (must be 0-{Pokemon.IVStatLimit}).\n{fileLineData.LineReport}"
                        );
                    }
                }

                if (pokemon.IV.Count < newIVs.Length)
                {
                    pokemon.IV.AddRange(newIVs[pokemon.IV.Count..]);
                }
            }

            if (pokemon.EV is not null)
            {
                var evTotal = 0;
                foreach (var stat in mainStats)
                {
                    newEVs[stat.PbsOrder] =
                        pokemon.EV.Count > stat.PbsOrder
                            ? pokemon.EV[stat.PbsOrder]
                            : pokemon.EV[0];
                    evTotal += newEVs[stat.PbsOrder];
                    if (newEVs[stat.PbsOrder] > Pokemon.EVStatLimit)
                    {
                        throw new PbsParseException(
                            $"Invalid EV: {newEVs[stat.PbsOrder]} (must be 0-{Pokemon.EVStatLimit}).\n{fileLineData.LineReport}"
                        );
                    }
                }

                if (pokemon.EV.Count < newEVs.Length)
                {
                    pokemon.EV.AddRange(newEVs[pokemon.EV.Count..]);
                }

                if (evTotal > Pokemon.EVLimit)
                {
                    throw new PbsParseException(
                        $"Invalid EV set (must sum to {Pokemon.EVLimit} or less)..\n{fileLineData.LineReport}"
                    );
                }
            }

            if (pokemon.Happiness is > Pokemon.MaxHappiness)
            {
                throw new PbsParseException(
                    $"Bad happiness: {pokemon.Happiness} (must be 0-{Pokemon.MaxHappiness})\n{fileLineData.LineReport}"
                );
            }

            if (pokemon.Ball.HasValue && !Item.Get(pokemon.Ball.Value).IsPokeBall)
            {
                throw new PbsParseException(
                    $"Value '{pokemon.Ball.Value}' isn't a defined Poké Ball.\n{fileLineData.LineReport}"
                );
            }
        }
    }

    public override async Task WriteToFile(
        PbsSerializer serializer,
        CancellationToken cancellationToken = default
    )
    {
        var schema = serializer.GetSchema(typeof(EnemyTrainerInfo)).ToDictionary();
        var subschema = serializer.GetSchema(typeof(TrainerPokemonInfo));

        await FileUtils.WriteFileWithBackupAsync(
            FileName,
            async fileWriter =>
            {
                await PbsSerializer.AddPbsHeaderToFile(fileWriter);

                foreach (var trainer in EnemyTrainer.Entities.Select(x => x.ToDto()))
                {
                    await fileWriter.WriteLineAsync("#-------------------------------");
                    await fileWriter.WriteLineAsync($"[{trainer.Id}]");

                    foreach (var (key, schemaEntry) in schema)
                    {
                        if (key == "SectionName")
                            continue;

                        var value = GetPropertyForPbs(trainer, key);
                        if (value is null)
                            continue;

                        await fileWriter.WriteAsync($"{key} = ");
                        await CsvWriter.WriteCsvRecord(value, fileWriter, schemaEntry);
                        await fileWriter.WriteLineAsync();
                    }

                    foreach (var (i, pokemon) in trainer.Pokemon.Index())
                    {
                        await fileWriter.WriteLineAsync(
                            $"Pokemon = {pokemon.Species},{pokemon.Level}"
                        );

                        foreach (var (key, schemaEntry) in subschema)
                        {
                            var value = GetPropertyForPbs(pokemon, key);
                            if (value is null)
                                continue;

                            await fileWriter.WriteAsync($"    {key} = ");
                            await CsvWriter.WriteCsvRecord(value, fileWriter, schemaEntry);
                            await fileWriter.WriteLineAsync();
                        }
                    }
                }
            }
        );
    }

    private static object? GetPropertyForPbs(TrainerPokemonInfo model, string key)
    {
        if (!_pokemonPropertyMap.TryGetValue(key, out var property))
        {
            property = typeof(TrainerPokemonInfo).GetProperty(key);
            ArgumentNullException.ThrowIfNull(property);
            _pokemonPropertyMap.Add(key, property);
        }

        var elementValue = property.GetValue(model);
        if (
            elementValue is false
            || (
                elementValue is IEnumerable enumerable
                && CollectionUtils.IsEmptyEnumerable(enumerable)
            )
        )
        {
            return null;
        }

        return key switch
        {
            nameof(TrainerPokemonInfo.Gender) => !Species.Get(model.Species).IsSingleGendered
                ? model.Gender?.ToString().ToLowerInvariant()
                : null,
            nameof(TrainerPokemonInfo.Shiny) when model.SuperShiny is true => null,
            _ => elementValue,
        };
    }
}
