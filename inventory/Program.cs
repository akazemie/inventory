using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace inventory
{
    internal class Program
    {
        private static ITelegramBotClient botClient;

        static async Task Main()
        {
            // Replace with your bot token
            botClient = new TelegramBotClient("7469215645:AAHLZIyhPeS1YJInXiwZkd827VHcVZX3-Eg");

            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");

            using var cts = new CancellationTokenSource();

            // Start receiving updates
            botClient.StartReceiving(
                new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), null, cts.Token);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            // Send cancellation request to stop bot
            cts.Cancel();
        }

        static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates
            if (update.Type != UpdateType.Message)
                return;

            var message = update.Message;

            if (message.Type == MessageType.Text)
            {
                Console.WriteLine($"Received a text message from {message.Chat.FirstName}: {message.Text}");

                await botClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    text: "You said:\n" + message.Text,
                    cancellationToken: cancellationToken
                );
            }
        }

        static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Log the error
            Console.WriteLine(exception.ToString());
            return Task.CompletedTask;
        }
    }
}