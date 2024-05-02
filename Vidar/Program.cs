using DSharpPlus;
using DSharpPlus.CommandsNext;
using Vidar;
using System;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Vidar
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = Secrets.DISCORD_TOKEN,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" }
            });

            commands.RegisterCommands<Lotto>();

            /*
                        var slash = discord.UseSlashCommands();

                        slash.RegisterCommands<Profile>();
                        slash.RegisterCommands<Lotto>();
                        slash.RegisterCommands<Items>();
            */
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}

//https://discord.com/api/oauth2/authorize?client_id=1184374468655726592&permissions=1099713047616&scope=bot