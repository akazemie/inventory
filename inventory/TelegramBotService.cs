using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace inventory
{
    public class TelegramBotService : ITelegramBotService
    {

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }

        private async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            if (message.Text == "/start")
            {
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Item management", "item_management"),
                    InlineKeyboardButton.WithCallbackData("Inventory management", "inventory_management"),
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

        private async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            InlineKeyboardMarkup inlineKeyboard = null;

            switch (callbackQuery.Data)
            {
                case "item_management":
                    inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Items list", "items_list"),
                        InlineKeyboardButton.WithCallbackData("Add Item", "add_item"),
                    }
                });
                    await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Item Management:",
                        replyMarkup: inlineKeyboard,
                        cancellationToken: cancellationToken
                    );
                    break;

                case "inventory_management":
                    inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Stockroom Items", "stockroom_items"),
                        InlineKeyboardButton.WithCallbackData("Add to stockroom", "add_to_stockroom"),
                    }
                });
                    await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Inventory Management:",
                        replyMarkup: inlineKeyboard,
                        cancellationToken: cancellationToken
                    );
                    break;

                case "items_list":
                    await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "You clicked 'Items list'.",
                        cancellationToken: cancellationToken
                    );
                    break;

                case "add_item":
                    await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "You clicked 'Add Item'.",
                        cancellationToken: cancellationToken
                    );
                    break;

                case "stockroom_items":
                    await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "You clicked 'Stockroom Items'.",
                        cancellationToken: cancellationToken
                    );
                    break;

                case "add_to_stockroom":
                    await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "You clicked 'Add to stockroom'.",
                        cancellationToken: cancellationToken
                    );
                    break;

                default:
                    await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Unknown action",
                        cancellationToken: cancellationToken
                    );
                    break;
            }

            await botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: "Processing your request...",
                cancellationToken: cancellationToken
            );
        }
    }

}
