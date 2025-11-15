using Injectio.Attributes;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;
using Zomp.SyncMethodGenerator;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton<IPbsCompiler>(Duplicate = DuplicateStrategy.Append)]
public partial class AbilityCompiler : PbsCompilerBase<AbilityInfo>
{
    public override int Order => 4;

    [CreateSyncVersion]
    public override async Task CompileAsync(PbsSerializer serializer, CancellationToken cancellationToken = default)
    {
        using var fileStream = new StreamReader(FileName);
        var fileLineData = new FileLineData(FileName);

        var entities = new List<AbilityInfo>();
        await foreach (
            var (sectionName, keysValueLines, lineData) in PbsSerializer.ParseFileSectionsAsync(
                fileStream,
                fileLineData,
                cancellationToken
            )
        )
        {
            Text? name;
            Text? description;
            List<string>? flags;
        }
    }

    [CreateSyncVersion]
    public override async Task WriteToFileAsync(PbsSerializer serializer, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private Ability ConvertToEntity(AbilityInfo model) => model.ToGameData();

    private AbilityInfo ConvertToModel(Ability entity) => entity.ToDto();
}
