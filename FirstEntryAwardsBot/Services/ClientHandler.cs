using Discord;
using Discord.WebSocket;
using Serilog;

namespace FirstEntryAwardsBot.Services
{
    public class ClientHandler
    {
        private readonly DiscordSocketClient _client;

        public ClientHandler(DiscordSocketClient client)
        {
            _client = client;
        }

#pragma warning disable 1998
        public async Task InitializeAsync ( )
#pragma warning restore 1998
        {
            _client.Ready += OnReady;
        }

        private async Task OnReady()
        {
            await _client.SetGameAsync("добычу ключей");
            await _client.SetStatusAsync(UserStatus.DoNotDisturb);
            Log.Information("{BotName}: Статус установлен!", _client.CurrentUser.Username);
        }
    }
}