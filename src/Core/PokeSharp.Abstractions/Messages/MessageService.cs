using PokeSharp.Core;
using PokeSharp.Core.Strings;
using PokeSharp.UI;

namespace PokeSharp.Messages;

[AutoServiceShortcut]
public interface IMessageService
{
    ITextWindow CreateTextWindow(IViewport? viewport = null, string? skin = null);

    ValueTask<ITextWindow> CreateTextWindowAsync(
        IViewport? viewport = null,
        string? skin = null,
        CancellationToken cancellationToken = default
    );

    void DisposeTextWindow(ITextWindow textWindow);

    ValueTask DisposeTextWindowAsync(ITextWindow textWindow, CancellationToken cancellationToken = default);

    ValueTask<T> MessageDisplay<T>(
        ITextWindow messageWindow,
        Text message,
        bool letterByLetter = true,
        Func<ITextWindow, CancellationToken, ValueTask<T>>? commandCallback = null,
        CancellationToken cancellationToken = default
    );

    ValueTask<int> ShowCommands(
        ITextWindow messageWindow,
        IEnumerable<Text> commands,
        int commandIfCancel = 0,
        int defaultCommand = 0,
        CancellationToken cancellationToken = default
    );

    ValueTask<int> ShowCommandsWithHelp(
        ITextWindow messageWindow,
        IEnumerable<(Text Command, Text Help)> commands,
        int commandIfCancel = 0,
        int defaultCommand = 0,
        CancellationToken cancellationToken = default
    );

    ValueTask<int> ChooseNumber(
        ITextWindow messageWindow,
        ChooseNumberParams @params,
        CancellationToken cancellationToken = default
    );
}
