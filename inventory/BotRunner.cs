using Telegram.Bot;
using Telegram.Bot.Polling;

namespace inventory
{
    public class BotRunner
    {
        private readonly ITelegramBotService _botService;
        private readonly ITelegramBotClient _botClient;

        public BotRunner(ITelegramBotService botService, ITelegramBotClient botClient)
        {
            _botService = botService;
            _botClient = botClient;
        }

        public async Task RunAsync()
        {
            var me = await _botClient.GetMeAsync();
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");

            using var cts = new CancellationTokenSource();

            _botClient.StartReceiving(
                new DefaultUpdateHandler(_botService.HandleUpdateAsync, _botService.HandleErrorAsync), null, cts.Token
            );

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            cts.Cancel();
        }
    }

}
