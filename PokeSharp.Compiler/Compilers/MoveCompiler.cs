using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Compilers;

public class MoveCompiler(ILogger<MoveCompiler> logger) : PbsCompiler<Move, MoveInfo>
{
    public override int Order => 5;

    protected override Move ConvertToEntity(MoveInfo model) => model.ToGameData();

    protected override MoveInfo ConvertToModel(Move entity) => entity.ToDto();

    protected override MoveInfo ValidateCompiledModel(MoveInfo model, FileLineData fileLineData)
    {
        if (model.Category == DamageCategory.Status && model.Power != 0)
        {
            throw new ValidationException(
                $"Move {model.Name} is defined as a Status move with a non-zero base damage.\n{fileLineData.LineReport}"
            );
        }

        if (model.Category == DamageCategory.Status || model.Power != 0)
            return model;

        logger.LogWarning(
            "Move {Name} is defined as Physical or Special but has a base damage of 0. Changing it to a Status move.\n{Line}",
            model.Name,
            fileLineData.LineReport
        );
        return model with { Category = DamageCategory.Status };
    }
}
