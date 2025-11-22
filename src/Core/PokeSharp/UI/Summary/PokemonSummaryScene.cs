using PokeSharp.Core;
using PokeSharp.PokemonModel;

namespace PokeSharp.UI.Summary;

public interface IPokemonSummaryScene : IScene
{
    SceneScope StartScene(IReadOnlyList<Pokemon> party, int partyIndex, bool inBattle = false);

    SceneScope StartForgetScene(IReadOnlyList<Pokemon> party, int partyIndex, Name? moveToLearn);

    ValueTask Display(Text text, CancellationToken cancellationToken = default);

    ValueTask<bool> Confirm(Text text, CancellationToken cancellationToken = default);

    ValueTask<int?> ShowCommands(
        IEnumerable<Text> commands,
        int index = 0,
        CancellationToken cancellationToken = default
    );

    ValueTask<int?> ChooseMoveToForget(Name? moveToLearn, CancellationToken cancellationToken = default);

    ValueTask<int> Scene(CancellationToken cancellationToken = default);
}
