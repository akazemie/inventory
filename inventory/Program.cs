using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace inventory
{
    internal class Program
    {
        private static ITelegramBotClient botClient;

        static async Task Main()
        {
            var botToken = Environment.GetEnvironmentVariable("Inventory_bot_token");
            botClient = new TelegramBotClient(botToken);

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
            if (update.Type == UpdateType.Message && update.Message.Type == MessageType.Text)
            {
                await HandleMessageAsync(botClient, update.Message, cancellationToken);
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallbackQueryAsync(botClient, update.CallbackQuery, cancellationToken);
            }
        }

        static async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            if (message.Text == "/start")
            {
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Show list", "show_list"),
                    InlineKeyboardButton.WithCallbackData("Add to Inventory", "add_inventory"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Edit Inventory", "edit_inventory"),
                    InlineKeyboardButton.WithCallbackData("Delete Inventory", "delete_inventory"),
                }
                });

                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Choose an option:",
                    replyMarkup: inlineKeyboard,
                    cancellationToken: cancellationToken
                );
            }
        }

        static async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            string response = callbackQuery.Data switch
            {
                "show_list" => "You clicked 'Show list'.",
                "add_inventory" => "You clicked 'Add to Inventory'.",
                "edit_inventory" => "You clicked 'Edit Inventory'.",
                "delete_inventory" => "You clicked 'Delete Inventory'.",
                _ => "Unknown action"
            };

            await botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: "Processing your request...",
                cancellationToken: cancellationToken
            );

            await botClient.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: response,
                cancellationToken: cancellationToken
            );
        }

        static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
    }
}