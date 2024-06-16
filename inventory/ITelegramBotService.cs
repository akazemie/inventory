using Telegram.Bot;
using Telegram.Bot.Types;

namespace inventory
{
    public interface ITelegramBotService
    {
        Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken);
    }
}
