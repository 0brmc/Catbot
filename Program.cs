using CatBot.Services;
using Discord;
using Discord.WebSocket;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Configuration;

namespace CatBot
{
    internal class Program
    {
        private DiscordSocketClient? _client;
        public static Task Main(string[] args) => new Program().MainAsync();

        public static string? token = ConfigurationManager.AppSettings["token"];

        public async Task MainAsync()
        {
            Console.ResetColor();

            Console.Title = "Catbot!";

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug, theme: AnsiConsoleTheme.Code)
            .Enrich.FromLogContext()
            .CreateLogger();

            Log.Information("CatBot!");
            Log.Information("Ligando bot...");

            if (token == null)
            {
                Log.Error("Faltando Token - Missing App.config");
                Console.ReadKey();
                Environment.Exit(1);
            }

            _client = new DiscordSocketClient();
            var _commandHandler = new CommandHandler(_client);

            _client.Log += (s) => Task.Run(() => Log.Information(s.Message));
            _client.Ready += () => Task.Run(() => _client_Ready().ConfigureAwait(false));
            _client.MessageReceived += (s) => Task.Run(() => _commandHandler.HandleCommandAsync(s).ConfigureAwait(false));

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1).ConfigureAwait(false);
        }

        private async Task _client_Ready()
        {
            var connectionState = _client.ConnectionState;
            if (connectionState == ConnectionState.Connected)
            {
                Log.Information("Bot Ligado! - Aguardando comandos...");
                await _client.SetGameAsync("Gatinhos! | !helpcat", null, ActivityType.Playing).ConfigureAwait(false);
                await _client.SetStatusAsync(UserStatus.Idle).ConfigureAwait(false);
            }
            else
            {
                Log.Error("Erro na conexão do bot");
            }
        }
    }
}