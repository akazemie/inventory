using Telegram.Bot;

namespace inventory
{
    internal class Program
    {
        static async Task Main()
        {
            var botToken = Environment.GetEnvironmentVariable("Inventory_bot_token");
            var botClient = new TelegramBotClient(botToken!);
            var botService = new TelegramBotService();
            var botRunner = new BotRunner(botService, botClient);

            await botRunner.RunAsync();
        }

    }
}