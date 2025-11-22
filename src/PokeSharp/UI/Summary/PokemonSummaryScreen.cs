using PokeSharp.Core;
using PokeSharp.Messages;
using PokeSharp.PokemonModel;

namespace PokeSharp.UI.Summary;

public class PokemonSummaryScreen(IPokemonSummaryScene scene, bool inBattle = false)
{
    private static readonly Text CantForgetHM = Text.Localized(
        "PokemonSummaryScreen",
        "CantForgetHM",
        "HM moves can't be forgotten now."
    );
    private static readonly Text YouMustChoose = Text.Localized(
        "PokemonSummaryScreen",
        "YouMustChoose",
        "You must choose a move!"
    );

    public async ValueTask<int> StartScreen(
        IReadOnlyList<Pokemon> party,
        int partyIndex,
        CancellationToken cancellationToken = default
    )
    {
        using var scope = scene.StartScene(party, partyIndex, inBattle);
        var result = await scene.Scene(cancellationToken);
        return result;
    }

    public async ValueTask<int?> StartForgetScreen(
        IReadOnlyList<Pokemon> party,
        int partyIndex,
        Name? moveToLearn,
        CancellationToken cancellationToken = default
    )
    {
        int? result = null;
        using var scope = scene.StartForgetScene(party, partyIndex, moveToLearn);
        while (!cancellationToken.IsCancellationRequested)
        {
            result = await scene.ChooseMoveToForget(moveToLearn, cancellationToken);
            if (result is null || moveToLearn is null || !party[partyIndex].Moves[result.Value].IsHiddenMove)
                break;

            await GameGlobal.MessageService.Message(CantForgetHM, cancellationToken: cancellationToken);
        }
        return result;
    }

    public async ValueTask<int?> StartChooseMoveScreen(
        IReadOnlyList<Pokemon> party,
        int partyIndex,
        Text message,
        CancellationToken cancellationToken = default
    )
    {
        int? result = null;
        using var scope = scene.StartForgetScene(party, partyIndex, null);
        await GameGlobal.MessageService.Message(message, cancellationToken: cancellationToken);
        while (!cancellationToken.IsCancellationRequested)
        {
            result = await scene.ChooseMoveToForget(null, cancellationToken);
            if (result is not null)
                break;

            await GameGlobal.MessageService.Message(YouMustChoose, cancellationToken: cancellationToken);
        }
        return result;
    }
}
