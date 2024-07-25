using DSharpPlus;
using DSharpPlus.CommandsNext;
using Vidar;
using System;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using DSharpPlus.Entities;
using DSharpPlus.AsyncEvents;
using DSharpPlus.CommandsNext.Executors;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;

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
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents | DiscordIntents.GuildMembers
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" },
                CommandExecutor = new AsynchronousCommandExecutor()
            }) ;

            discord.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromMinutes(5)
            });

            commands.RegisterCommands<Lotto>();
            commands.RegisterCommands<Profile>();

            discord.GuildMemberAdded += MemberAddedHandler;

            Task MemberAddedHandler(DiscordClient s, GuildMemberAddEventArgs e)
            {
                e.Guild.GetChannel(1235322443237818511).SendMessageAsync($"Welcome {e.Member.Mention}! Please use `!register <Cartel Empire ID>` to register your ID with the bot.");
                return Task.CompletedTask;
            }

            commands.CommandErrored += Commands_CommandErrored;
            discord.ClientErrored += Discord_ClientErrored;

            /*
            var slash = discord.UseSlashCommands();

            slash.RegisterCommands<Profile>();
            slash.RegisterCommands<Lotto>();
            slash.RegisterCommands<Items>();
            */
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task Discord_ClientErrored(DiscordClient sender, ClientErrorEventArgs args)
        {
            Console.WriteLine(args.Exception);
            return Task.CompletedTask;
        }

        private static Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs args)
        {
            Console.WriteLine(args.Exception);
            return Task.CompletedTask;
        }
    }
}

//https://discord.com/oauth2/authorize?client_id=1235655726911852586&permissions=268577792&integration_type=0&scope=bot