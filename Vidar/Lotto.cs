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

namespace Vidar
{
    internal class Lotto : BaseCommandModule
    {
        List<DiscordUser> lottoEntries = new List<DiscordUser>();
        bool lottoLock = false;
        bool lottoDrawn = false;
        bool prizeSent = false;
        string lottoPrize = "";
        static Random rnd = new Random();
        DiscordUser? lottoRunner;

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
                //DiscordRole lottoPing = ctx.Guild.GetRole(1235663172006973460);
                lottoLock = true;
                lottoEntries.Clear();
                lottoPrize = prize;

                string lottoStartMessage = $"Hey, {lottoPing.Mention}!" + System.Environment.NewLine + $"A lotto has started for {prize}! Join now with `!j`!";

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

        [Command("join")]
        [Aliases("j")]
        public async Task JoinLottoCommand(CommandContext ctx)
        {
            DiscordRole lottoPing = ctx.Guild.GetRole(1235663172006973460);
            if (ctx.User == lottoRunner)
            {
                await ctx.RespondAsync("You cannot join your own lotto.");
            }
            else if (!ctx.Member.Roles.Contains(lottoPing))
            {
                await ctx.RespondAsync("You are not registered with the bot. Please use `!register <Cartel Empire ID>` to register your ID with the bot.");
            } else
            {
                if (lottoLock && !lottoDrawn)
                {
                    await ctx.TriggerTypingAsync();
                    string response = "";
                    if (lottoEntries.Contains(ctx.User))
                    {
                        response = $"{ctx.User.Mention} has already entered the lotto.";
                    }
                    else
                    {
                        lottoEntries.Add(ctx.User);
                        response = $"{ctx.User.Mention} has entered the lotto for {lottoPrize}.";
                    }
                    await ctx.RespondAsync(response);
                }
                else
                {
                    await ctx.RespondAsync("No active lotto found");
                }
            }
        }

        [Command("draw")]
        public async Task DrawLottoCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            long winner_id = 0;

            if (ctx.User == lottoRunner)
            {
                lottoDrawn = true;
                int r = rnd.Next(lottoEntries.Count);
                DiscordUser winner = lottoEntries[r];

                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
                {
                    Server = Secrets.SQL_SERVER,
                    UserID = Secrets.SQL_USER,
                    Password = Secrets.SQL_PASSWORD,
                    Database = Secrets.SQL_DATABASE,
                };

                // open a connection asynchronously
                using MySqlConnection connection = new MySqlConnection(builder.ConnectionString);
                try {
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
                } catch (MySqlException ex) {
                    await ctx.Channel.SendMessageAsync("SQL connection failed in report." + Environment.NewLine + ex.Message);
                }
                await ctx.RespondAsync($"Congratulations {winner.Mention}[{winner_id}]! You won the lotto for {lottoPrize}!");

                await listen(ctx);
            } else
            {
                await ctx.RespondAsync($"Only the lotto runner can end the lotto.");
            }
        }

        public async Task listen(CommandContext ctx)
        {
            InteractivityResult<DiscordMessage> result = await ctx.Message.GetNextMessageAsync(message =>
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
                lottoEntries.Clear();
            } else
            {
                await ctx.TriggerTypingAsync();
                await ctx.RespondAsync($"Shame on you,{lottoRunner.Mention}...where is the send line?");
                await listen(ctx);
            }
        }

        [Command("terminate")] 
        [RequireOwnerAttribute]
        public async Task TerminateLottoCommand(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            lottoLock = false;
            lottoDrawn = false;
            prizeSent = false;
            lottoEntries.Clear();
            await ctx.RespondAsync("Lotto terminated.");
        }
    }
}
