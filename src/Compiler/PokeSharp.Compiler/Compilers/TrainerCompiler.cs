using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using Injectio.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Compiler.Core.Utils;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Core.Utils;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;
using PokeSharp.PokemonModel;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class TrainerCompiler(ILogger<TrainerCompiler> logger, IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings)
    : PbsCompiler<EnemyTrainer, EnemyTrainerInfo>(logger, pbsCompileSettings)
{
    public override int Order => 15;

    protected override EnemyTrainer ConvertToEntity(EnemyTrainerInfo model)
    {
        return model.ToGameData();
    }

    protected override EnemyTrainerInfo ConvertToModel(EnemyTrainer entity)
    {
        return entity.ToDto();
    }

    protected override void ValidateCompiledModel(EnemyTrainerInfo trainer, FileLineData fileLineData)
    {
        if (trainer.Pokemon.Count == 0)
        {
            throw new PbsParseException($"Trainer with ID '{trainer.Id}' has no Pokémon.\n{fileLineData.LineReport}");
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

            if (pokemon.Name.HasValue && pokemon.Name.Value.AsReadOnlySpan().Length > Pokemon.NameSizeLimit)
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
                        pokemon.IV.Count > stat.PbsOrder ? pokemon.IV[stat.PbsOrder] : pokemon.IV[0];
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
                        pokemon.EV.Count > stat.PbsOrder ? pokemon.EV[stat.PbsOrder] : pokemon.EV[0];
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
}
