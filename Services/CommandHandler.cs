using Discord;
using Discord.WebSocket;
using Serilog;

namespace CatBot.Services
{
    public class CommandHandler
    {
        IDisposable? typing;
        private readonly string[] commands = new string[] { "!helpcat", "!cat", "!catlist", "!flood", "!banana", "!dado", "!coinflip", "!meme" };
        private readonly string[] keywords = new string[] { "puto", "vava" };

        private DiscordSocketClient _client;
        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task HandleCommandAsync(SocketMessage arg)
        {
            if (arg is not SocketUserMessage message) return;
            if (message.Source != MessageSource.User) return;

            try
            {
                Log.Debug(DateTime.Now.ToString("HH:mm:ss") + " " + message.Author + " " + message.Channel + " " + message.CleanContent);
                ISocketMessageChannel? channel = message.Channel;
                bool hasArguments = HasArguments(message.CleanContent);

                foreach (var word in message.CleanContent.ToLower().Split(' '))
                {
                    //Trata os commandos separadamente
                    if (commands.Any(word.Contains) || keywords.Any(word.Contains))
                    {
                        typing = message.Channel.EnterTypingState();
                        HttpClient httpClient = new HttpClient();
                        switch (word)
                        {
                            case "!helpcat":
                                var stringToSend = "--Lista de comandos: ";
                                foreach (var command in commands)
                                    stringToSend += $"\n{command}";

                                stringToSend += "\n--Lista de keywords: ";
                                foreach (var keyword in keywords)
                                    stringToSend += $"\n{keyword}";

                                await channel.SendMessageAsync(stringToSend, messageReference: new MessageReference(message.Id));
                                break;

                            case "!cat":
                                if (!hasArguments)
                                {
                                    await channel.SendFileAsync(new FileAttachment(await CatPicService.GetRandomCatPictureAsync(), "cat.png"), "", messageReference: new MessageReference(message.Id));
                                    break;
                                }

                                var pic = await CatPicService.GetRandomCatPictureAsync(message.Content.Split(' ')[1]);
                                if (pic != null)
                                    await channel.SendFileAsync(new FileAttachment(pic, "cat.png"), "", messageReference: new MessageReference(message.Id));

                                break;

                            case "!catlist":
                                await channel.SendMessageAsync("https://cataas.com/api/tags", messageReference: new MessageReference(message.Id));
                                break;

                            case "!flood":
                                if (message.MentionedUsers.Count == 0)
                                {
                                    await channel.SendMessageAsync("Marque quem deseja floodar.", messageReference: new MessageReference(message.Id));
                                    break;
                                }

                                await foreach (var user in message.MentionedUsers.ToAsyncEnumerable())
                                {
                                    for (int i = 0; i < 10; i++)
                                        await channel.SendMessageAsync(user.Mention, allowedMentions: AllowedMentions.All);
                                }
                                await channel.SendMessageAsync("Floodado.", messageReference: new MessageReference(message.Id));
                                break;

                            case "!banana":
                                var response = await httpClient.GetAsync("https://s.yimg.com/uu/api/res/1.2/yaEm82sJN0n7q6R3SpGXTQ--~B/aD03MjA7dz0xMjgwO2FwcGlkPXl0YWNoeW9u/http://media.zenfs.com/pt-BR/video/video.br.rumble.com/efb0c19b927fba16cb08f3cda045c542");
                                var content = await response.Content.ReadAsStreamAsync();
                                await channel.SendFileAsync(new FileAttachment(content, "banana.png"), "b A n A n A", messageReference: new MessageReference(message.Id));
                                break;

                            case "puto":
                                await channel.SendMessageAsync("Toma suquinho de maracujá", messageReference: new MessageReference(message.Id));
                                break;

                            case "vava":
                                await channel.SendMessageAsync("Ah, o jogo que ainda tá no beta?", messageReference: new MessageReference(message.Id));
                                break;

                            case "!coinflip":
                                if (new Random().Next(1, 3) == 1)
                                {
                                    await channel.SendMessageAsync("Cara 🤠", messageReference: new MessageReference(message.Id));
                                    break;
                                }

                                await channel.SendMessageAsync("Coroa 👑", messageReference: new MessageReference(message.Id));
                                break;

                            case "!meme":
                                if (!hasArguments)
                                {
                                    await channel.SendMessageAsync("Escreva o texto a ser inserido na imagem.", messageReference: new MessageReference(message.Id));
                                    break;
                                }

                                var image = await httpClient.GetAsync(message.Attachments.First().Url).GetAwaiter().GetResult().Content.ReadAsStreamAsync();

                                await channel.SendFileAsync(new FileAttachment(ImageService.DrawTextonImage(message.Content.Split(' ')[1], image), "meme.png"), "", messageReference: new MessageReference(message.Id));
                                break;

                            case "!dado":
                                var dice = "";
                                var number = 0;
                                switch (new Random().Next(1, 7))
                                {
                                    case 1:
                                        dice = "🔲🔲🔲\n" +
                                               "🔲🔳🔲\n" +
                                               "🔲🔲🔲";
                                        number = 1;
                                        break;
                                    case 2:
                                        dice = "🔲🔲🔳\n" +
                                               "🔲🔲🔲\n" +
                                               "🔳🔲🔲";
                                        number = 2;
                                        break;
                                    case 3:
                                        dice = "🔲🔲🔳\n" +
                                               "🔲🔳🔲\n" +
                                               "🔳🔲🔲";
                                        number = 3;
                                        break;
                                    case 4:
                                        dice = "🔳🔲🔳\n" +
                                               "🔲🔲🔲\n" +
                                               "🔳🔲🔳";
                                        number = 4;
                                        break;
                                    case 5:
                                        dice = "🔳🔲🔳\n" +
                                               "🔲🔳🔲\n" +
                                               "🔳🔲🔳";
                                        number = 5;
                                        break;
                                    case 6:
                                        dice = "🔳🔲🔳\n" +
                                               "🔳🔲🔳\n" +
                                               "🔳🔲🔳";
                                        number = 6;
                                        break;
                                }

                                await channel.SendMessageAsync(dice + "\n" + number.ToString(), messageReference: new MessageReference(message.Id));
                                break;
                        }
                        typing.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                if (typing is not null)
                {
                    typing.Dispose();
                }

                Log.Error(ex.ToString());
            }
        }

        static bool HasArguments(string message)
        {
            try
            {
                string? argument = message.Split(' ')[1];
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
            return true;
        }
    }
}
