using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Controllers;

public interface ISelectableController
{
    IEnumerable<Text> GetLabels();
}