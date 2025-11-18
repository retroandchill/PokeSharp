using PokeSharp.Core;

namespace PokeSharp.Messages;

public static class MessageExtensions
{
    private static readonly Text YesText = Text.Localized("Messages", "Yes", "Yes");
    private static readonly Text NoText = Text.Localized("Messages", "No", "No");

    extension(IMessageService service)
    {
        public async ValueTask Message(Text message, string? skin = null, CancellationToken cancellationToken = default)
        {
            var messageWindow = await service.CreateTextWindowAsync(null, skin, cancellationToken);
            await service.MessageDisplay<int>(messageWindow, message, cancellationToken: cancellationToken);
            await service.DisposeTextWindowAsync(messageWindow, cancellationToken);
        }

        public async ValueTask<int> Message(
            Text message,
            IEnumerable<Text> commands,
            int commandIfCancel = 0,
            string? skin = null,
            int defaultCommand = 0,
            CancellationToken cancellationToken = default
        )
        {
            var messageWindow = await service.CreateTextWindowAsync(null, skin, cancellationToken);
            var result = await service.MessageDisplay(
                messageWindow,
                message,
                commandCallback: async (w, c) =>
                    await service.ShowCommands(w, commands, commandIfCancel, defaultCommand, c),
                cancellationToken: cancellationToken
            );
            await service.DisposeTextWindowAsync(messageWindow, cancellationToken);
            return result;
        }

        public async ValueTask<bool> ConfirmMessage(Text text, CancellationToken cancellationToken = default)
        {
            return await service.Message(text, [YesText, NoText], 2, cancellationToken: cancellationToken) == 0;
        }

        public async ValueTask<bool> ConfirmMessageSerious(Text text, CancellationToken cancellationToken = default)
        {
            return await service.Message(text, [NoText, YesText], 1, cancellationToken: cancellationToken) == 1;
        }

        public async ValueTask<int> MessageChooseNumber(
            Text message,
            ChooseNumberParams @params,
            CancellationToken cancellationToken = default
        )
        {
            var messageWindow = await service.CreateTextWindowAsync(null, @params.MessageSkin, cancellationToken);
            var result = await service.MessageDisplay(
                messageWindow,
                message,
                commandCallback: async (w, c) => await service.ChooseNumber(w, @params, c),
                cancellationToken: cancellationToken
            );
            await service.DisposeTextWindowAsync(messageWindow, cancellationToken);
            return result;
        }
    }
}
