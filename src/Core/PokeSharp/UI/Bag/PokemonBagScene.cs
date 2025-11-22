using PokeSharp.Core;
using PokeSharp.Items;

namespace PokeSharp.UI.Bag;

public interface IPokemonBagScene : IScene
{
    SceneScope StartScene(
        PokemonBag bag,
        bool choosing = false,
        Func<Name, bool>? filter = null,
        bool resetPocket = false
    );

    ValueTask Display(Text text, CancellationToken cancellationToken = default);

    ValueTask<bool> Confirm(Text helpText, CancellationToken cancellationToken = default);

    ValueTask<int> ChooseNumber(
        Text helpText,
        int maximum,
        int initial = 1,
        CancellationToken cancellationToken = default
    );

    ValueTask<int?> ShowCommands(
        Text helpText,
        IReadOnlyList<Text> commands,
        int index = 0,
        CancellationToken cancellationToken = default
    );

    void Refresh();

    void RefreshIndexChanged();

    void RefreshFilter();

    ValueTask<Name?> ChooseItem(CancellationToken cancellationToken = default);
}
