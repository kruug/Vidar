using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using MySqlConnector;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vidar
{
    internal class Profile : BaseCommandModule
    {
        [Command("register")]
        public async Task RegisterCommand(CommandContext ctx, int UserID)
        {
            await ctx.TriggerTypingAsync();
            string desc;
            DiscordRole lottoUsers = ctx.Guild.GetRole(1235663172006973460);

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
            {
                Server = Secrets.SQL_SERVER,
                UserID = Secrets.SQL_USER,
                Password = Secrets.SQL_PASSWORD,
                Database = Secrets.SQL_DATABASE,
            };

            using MySqlConnection connection = new MySqlConnection(builder.ConnectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO playerregister(discord_id, user_id) VALUES (@user, @key);";
            command.Parameters.AddWithValue("@user", ctx.User.Id);
            command.Parameters.AddWithValue("@key", UserID);

            var dbresult = await command.ExecuteNonQueryAsync();

            if (dbresult > 0)
            {
                desc = $"{ctx.User.Mention}, your ID has been sucessfully registered as [{UserID}]";
                await ctx.Member.GrantRoleAsync(lottoUsers);
            }
            else
            {
                desc = "Failed to submit your user ID. Please contact kruug[3573] to resolve the issue.";
            }

            await ctx.RespondAsync(desc);
        }
    }
}
