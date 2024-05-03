using DSharpPlus;
using DSharpPlus.CommandsNext;
using Vidar;
using System;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
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

            discord.GuildMemberAdded += MemberAddedHandler;

            Task MemberAddedHandler(DiscordClient s, GuildMemberAddEventArgs e)
            {
                DiscordRole lottoUsers = e.Guild.GetRole(1235663172006973460);
                e.Member.GrantRoleAsync(lottoUsers);
                return Task.CompletedTask;
            }

            commands.CommandErrored += Commands_CommandErrored;

            /*
                        var slash = discord.UseSlashCommands();

                        slash.RegisterCommands<Profile>();
                        slash.RegisterCommands<Lotto>();
                        slash.RegisterCommands<Items>();
            */
            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs args)
        {
            Console.WriteLine(args.Exception);
            return Task.CompletedTask;
        }
    }
}

//https://discord.com/api/oauth2/authorize?client_id=1184374468655726592&permissions=1099713047616&scope=bot