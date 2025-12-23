using Injectio.Attributes;
using PokeSharp.Core.Strings;
using PokeSharp.Data.Pbs;
using PokeSharp.Editor.Core;
using PokeSharp.Editor.Core.PokeEdit.Controllers;
using PokeSharp.Editor.Core.PokeEdit.Editors;
using PokeSharp.Editor.Core.PokeEdit.Requests;

namespace PokeSharp.Editor.Controllers;

[RegisterSingleton(ServiceType = typeof(IPokeEditController), Duplicate = DuplicateStrategy.Append)]
[PokeEditController]
public partial class TypeController(IEntityRepository<Name, PokemonType> repository) : EntityControllerBase<Name, PokemonType>(repository), ISelectableController
{
    [PokeEditRequest]
    public IEnumerable<Text> GetLabels()
    {
        return Repository.Entries.Values.Select(x => x.Name);
    }
}