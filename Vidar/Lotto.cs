using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus;
using System.Reflection;
using DSharpPlus.Interactivity.Extensions;
using System.Diagnostics;
using System.Linq.Expressions;
using DSharpPlus.EventArgs;
using MySqlConnector;
using System.Drawing;

namespace Vidar
{
    internal class Lotto : BaseCommandModule
    {
        List<DiscordMember> lottoEntries = new List<DiscordMember>();
        bool autodraw = false;
        bool lastcall = false;
        bool lottoLock = false;
        bool lottoDrawn = false;
        bool prizeSent = false;
        string lottoPrize = "";
        string lottoType = "regular";
        static Random rnd = new Random();
        DiscordUser? lottoRunner;
        InteractivityResult<DiscordMessage> result;
        DiscordColor WineRed = new DiscordColor(166, 17, 86);
        DiscordColor FaeGreen = new DiscordColor(44, 128, 106);

        [Command("startlotto")]
        [Aliases("sl")]
        public async Task StartLottoCommand(CommandContext ctx, [RemainingText] string prize)
        {
            DiscordRole lottoPing = ctx.Guild.GetRole(1235663172006973460);

            if (!ctx.Member.Roles.Contains(lottoPing))
            {
                await ctx.RespondAsync("You are not registered with the bot. Please use `!register <Cartel Empire ID>` to register your ID with the bot.");
            } else if (!lottoLock)
            {
                await ctx.TriggerTypingAsync();
                lottoRunner = ctx.User;
                lottoLock = true;
                lottoEntries.Clear();
                lottoPrize = prize;
                lottoType = "regular";

                string lottoStartMessage = $"Hey, {lottoPing.Mention}!" + System.Environment.NewLine + $"A lotto has started for {prize}! Use `!j` to join the lotto!";

                var msg = await new DiscordMessageBuilder()
                    .WithContent(lottoStartMessage)
                    .WithAllowedMentions(new IMention[] { new RoleMention(lottoPing) })
                    .SendAsync(ctx.Channel);
            }
            else
            {
                await ctx.RespondAsync("Please wait until the current lotto is completed.");
            }
        }

        [Command("startnooblotto")]
        [Aliases("snl")]
        public async Task StartNoobLottoCommand(CommandContext ctx, [RemainingText] string prize)
        {
            DiscordRole lottoPing = ctx.Guild.GetRole(1235663172006973460);
            DiscordRole nooblotto = ctx.Guild.GetRole(1294302632311132190);

            if (!ctx.Member.Roles.Contains(lottoPing))
            {
                await ctx.RespondAsync("You are not registered with the bot. Please use `!register <Cartel Empire ID>` to register your ID with the bot.");
            }
            else if (!lottoLock)
            {
                await ctx.TriggerTypingAsync();
                lottoRunner = ctx.User;
                lottoLock = true;
                lottoEntries.Clear();
                lottoPrize = prize;
                lottoType = "noob";

                string lottoStartMessage = $"Hey, {nooblotto.Mention}!" + System.Environment.NewLine + $"A lotto has started for {prize}! Use `!j` to join the lotto!";

                var msg = await new DiscordMessageBuilder()
                    .WithContent(lottoStartMessage)
                    .WithAllowedMentions(new IMention[] { new RoleMention(nooblotto) })
                    .SendAsync(ctx.Channel);
            }
            else
            {
                await ctx.RespondAsync("Please wait until the current lotto is completed.");
            }
        }

        [Command("join")]
        [Aliases("j")]
        public async Task JoinLottoCommand(CommandContext ctx)
        {
            DiscordRole lottoPing = ctx.Guild.GetRole(1235663172006973460);
            DiscordRole nooblotto = ctx.Guild.GetRole(1294302632311132190);

            if (lottoType == "regular")
            {
                if (ctx.User == lottoRunner)
                {
                    await ctx.Channel.SendMessageAsync("You cannot join your own lotto.");
                }
                else if (!ctx.Member.Roles.Contains(lottoPing)) {
                    await ctx.Channel.SendMessageAsync($"{ctx.User.Mention}, you are not registered with the bot. Please use `!register <Cartel Empire ID>` to register your ID with the bot.");
                }
                else
                {
                    if (lottoLock && !lottoDrawn)
                    {
                        await ctx.TriggerTypingAsync();
                        string response = "";
                        DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
                        if (lottoEntries.Contains(ctx.User))
                        {
                            embed.Color = WineRed;
                            int lottoindex = lottoEntries.IndexOf(ctx.Member);
                            response = $"{ctx.User.Mention} has already entered the lotto. (Entry #{lottoindex})";
                        }
                        else
                        {
                            embed.Color = FaeGreen;
                            lottoEntries.Add(ctx.Member);
                            int lottoindex = lottoEntries.IndexOf(ctx.Member);
                            response = $"{ctx.User.Mention} has entered the lotto for **{lottoPrize}**. (Entry #{lottoindex})";
                        }
                        embed.Description = response;
                        //await ctx.Channel.SendMessageAsync(response);
                        await ctx.Channel.SendMessageAsync(embed);
                    }
                    else
                    {
                        await ctx.Channel.SendMessageAsync("No active lotto found");
                    }
                }
            } else {
                if (ctx.User == lottoRunner)
                {
                    await ctx.Channel.SendMessageAsync("You cannot join your own lotto.");
                }
                else if (!ctx.Member.Roles.Contains(nooblotto)) {
                    await ctx.Channel.SendMessageAsync($"{ctx.User.Mention}, you are not a youngling.");
                }
                else
                {
                    if (lottoLock && !lottoDrawn)
                    {
                        await ctx.TriggerTypingAsync();
                        string response = "";
                        DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
                        if (lottoEntries.Contains(ctx.User))
                        {
                            embed.Color = WineRed;
                            int lottoindex = lottoEntries.IndexOf(ctx.Member);
                            response = $"{ctx.User.Mention} has already entered the lotto. (Entry #{lottoindex})";
                        }
                        else
                        {
                            embed.Color = FaeGreen;
                            lottoEntries.Add(ctx.Member);
                            int lottoindex = lottoEntries.IndexOf(ctx.Member);
                            response = $"{ctx.User.Mention} has entered the lotto for **{lottoPrize}**. (Entry #{lottoindex})";
                        }
                        embed.Description = response;
                        //await ctx.Channel.SendMessageAsync(response);
                        await ctx.Channel.SendMessageAsync(embed);
                    }
                    else
                    {
                        await ctx.Channel.SendMessageAsync("No active lotto found");
                    }
                }
            }
            await ctx.Channel.DeleteMessageAsync(ctx.Message);
        }

        [Command("draw")]
        public async Task DrawLottoCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            if (ctx.User == lottoRunner)
            {
                if (!autodraw)
                {
                    await DrawSubroutine(ctx);
                }
                else
                {
                    await ctx.RespondAsync($"Autodraw is already set.");
                }
            } else
            {
                await ctx.RespondAsync($"Only the lotto runner can end the lotto.");
            }
        }

        [Command("fdraw")]
        [RequirePermissions(permissions: Permissions.ManageMessages)]
        public async Task ForceDrawLottoCommand(CommandContext ctx)
        {
            await DrawSubroutine(ctx);
        }

        [Command("autodraw")]
        public async Task AutoDrawCommand(CommandContext ctx, [RemainingText] int countdown = 5)
        {
            if (ctx.User == lottoRunner)
            {
                autodraw = true;

                await ctx.Channel.SendMessageAsync($"The lotto will be drawn in {countdown} minutes.");
                await TimeDelayDraw(ctx, (countdown * 60));
            } else
            {
                await ctx.RespondAsync($"Only the lotto runner can end the lotto.");
            }

        }

        [Command("lastcall")]
        [Aliases("lc")]
        public async Task LastCallCommand(CommandContext ctx)
        {
            DiscordRole lottoPing = ctx.Guild.GetRole(1235663172006973460);
            DiscordRole nooblotto = ctx.Guild.GetRole(1294302632311132190);

            if (ctx.User == lottoRunner)
            {
                if (!lastcall)
                {
                    if (lottoType == "regular")
                    {
                        await ctx.Channel.SendMessageAsync($"Last Call! {lottoPing.Mention}! The lotto will be drawn soon!");
                    } else
                    {
                        await ctx.Channel.SendMessageAsync($"Last Call! {nooblotto.Mention}! The lotto will be drawn soon!");
                    }
                    lastcall = true;
                }
                else
                {
                    await ctx.RespondAsync($"Last call was already called.");
                }
            }
            else
            {
                await ctx.RespondAsync($"Only the lotto runner can use lotto commands.");
            }
        }

        [Command("countdown")]
        [Aliases("cd")]
        public async Task CountDownCommand(CommandContext ctx)
        {
            if (ctx.User == lottoRunner)
            {
                if (!autodraw)
                {
                    DiscordRole lottoPing = ctx.Guild.GetRole(1235663172006973460);
                    await ctx.Channel.SendMessageAsync($"Last Call! {lottoPing.Mention}! The lotto will be drawn in 15 seconds");
                    await TimeDelayDraw(ctx, 15);
                } else
                {
                    await ctx.RespondAsync($"Autodraw is already set.");
                }
            } else
            {
                await ctx.RespondAsync($"Only the lotto runner can end the lotto.");
            }
        }

        public async Task listen(CommandContext ctx)
        {
            result = await ctx.Message.GetNextMessageAsync(message =>
            {
                return message.Content.ToLower().Substring(0, 8) == "you sent";
            });

            if (!result.TimedOut)
            {
                await ctx.TriggerTypingAsync();
                await ctx.RespondAsync($"Thank you for the lotto,{lottoRunner.Mention}!");
                lottoLock = false;
                lottoDrawn = false;
                prizeSent = false;
                autodraw = false;
                lastcall = false;
                lottoEntries.Clear();
            } else
            {
                if (!lottoLock) { return; }
                await ctx.TriggerTypingAsync();
                await ctx.RespondAsync($"Shame on you,{lottoRunner.Mention}...where is the send line?");
                await listen(ctx);
            }
        }

        [Command("terminate")]
        [RequirePermissions(permissions: Permissions.ManageMessages)]
        public async Task TerminateLottoCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            lottoLock = false;
            lottoDrawn = false;
            prizeSent = false;
            autodraw = false;
            lastcall = false;
            lottoEntries.Clear();
            //result = new InteractivityResult<DiscordMessage>();
            await ctx.RespondAsync("Lotto terminated.");
        }

        public async Task TimeDelayDraw(CommandContext ctx, int seconds)
        {
            CancellationTokenSource source = new CancellationTokenSource();

            var t = Task.Run(async delegate
            {
                await Task.Delay(TimeSpan.FromSeconds(seconds), source.Token);
                await DrawSubroutine(ctx);
            });

            try
            {
                t.Wait();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                    Console.WriteLine("{0}: {1}", e.GetType().Name, e.Message);
            }
            source.Dispose();
        }

        public async Task DrawSubroutine(CommandContext ctx)
        {
            Profile x = new Profile();
            long winner_id = 0;
            lottoDrawn = true;
            int r = rnd.Next(lottoEntries.Count);
            DiscordMember winner = lottoEntries[r];

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
            {
                Server = Secrets.SQL_SERVER,
                UserID = Secrets.SQL_USER,
                Password = Secrets.SQL_PASSWORD,
                Database = Secrets.SQL_DATABASE,
            };

            // open a connection asynchronously
            using MySqlConnection connection = new MySqlConnection(builder.ConnectionString);
            try
            {
                await connection.OpenAsync();

                using MySqlCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT user_id FROM playerregister WHERE discord_id = '" + winner.Id + "';";
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        winner_id = reader.GetInt32(0);
                    }
                }
                reader.Close();
            }
            catch (MySqlException ex)
            {
                await ctx.Channel.SendMessageAsync("SQL connection failed in report." + Environment.NewLine + ex.Message);
            }
            lottoType = "regular";

            await ctx.RespondAsync($"Congratulations {winner.Mention}[{winner_id}]! You won the lotto for {lottoPrize} from {lottoRunner.Mention}!");
            await x.ProfileCommand(ctx, winner);

            await listen(ctx);
        }
    }
}
