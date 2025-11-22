using System.ComponentModel.DataAnnotations;
using Injectio.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Core;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class MoveCompiler(ILogger<MoveCompiler> logger, IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings)
    : PbsCompiler<Move, MoveInfo>(logger, pbsCompileSettings)
{
    public override int Order => 5;

    protected override Move ConvertToEntity(MoveInfo model) => model.ToGameData();

    protected override MoveInfo ConvertToModel(Move entity) => entity.ToDto();

    protected override void ValidateCompiledModel(MoveInfo model, FileLineData fileLineData)
    {
        if (model.Category == DamageCategory.Status && model.Power != 0)
        {
            throw new ValidationException(
                $"Move {model.Name} is defined as a Status move with a non-zero base damage.\n{fileLineData.LineReport}"
            );
        }

        if (model.Category == DamageCategory.Status || model.Power != 0)
            return;

        Logger.LogMoveChangedToSpecial(model.Name, fileLineData.LineReport);
        model.Category = DamageCategory.Status;
    }
}

internal static partial class MoveCompilerLogs
{
    [LoggerMessage(
        LogLevel.Warning,
        "Move {Name} is defined as Physical or Special but has a base damage of 0. Changing it to a Status move.\n{Line}"
    )]
    public static partial void LogMoveChangedToSpecial(
        this ILogger<PbsCompiler<Move, MoveInfo>> logger,
        Text name,
        string line
    );
}
