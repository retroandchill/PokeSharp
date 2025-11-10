using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Compilers;

public class TrainerTypeCompiler : PbsCompiler<TrainerType, TrainerTypeInfo>
{
    public override int Order => 14;

    protected override TrainerType ConvertToEntity(TrainerTypeInfo model) => model.ToGameData();

    protected override TrainerTypeInfo ConvertToModel(TrainerType entity) => entity.ToDto();
}
