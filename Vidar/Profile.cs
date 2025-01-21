using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using MySqlConnector;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Globalization;
using DSharpPlus.Interactivity;
using System.Drawing;
using DSharpPlus;

namespace Vidar
{
    internal class Profile : BaseCommandModule
    {
        DiscordColor Greyish = new DiscordColor(137, 137, 137);

        [Command("register")]
        public async Task RegisterCommand(CommandContext ctx, int UserID)
        {
            await ctx.TriggerTypingAsync();
            Color WineRed = Color.FromArgb(166, 17, 86);
            Color FaeGreen = Color.FromArgb(44, 128, 106);
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

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

            if (dbresult > 0)
            {
                embed.Title = "Registration Successful";
                embed.Color = new DiscordColor(FaeGreen.R, FaeGreen.G, FaeGreen.B);
                embed.Description = $"{ctx.User.Mention}, your ID has been sucessfully registered as [{UserID}]";

                desc = $"{ctx.User.Mention}, your ID has been sucessfully registered as [{UserID}]";
                await ctx.Member.GrantRoleAsync(lottoUsers);
            }
            else
            {
                embed.Title = "Registration Failed";
                embed.Color = new DiscordColor(WineRed.R,WineRed.G, WineRed.B);
                embed.Description = "Failed to submit your user ID. Please contact kruug[3573] to resolve the issue.";

                desc = "Failed to submit your user ID. Please contact kruug[3573] to resolve the issue.";
            }

            //await ctx.RespondAsync(desc);
            await ctx.RespondAsync(embed);
        }

        [Command("profile")]
        [Aliases("p")]
        public async Task ProfileCommand(CommandContext ctx, DiscordMember? target = null)
        {
            await ctx.TriggerTypingAsync();
            string disc_user_id = "blank";
            string ce_user_id = "blank";
            if (target == null)
            {
                disc_user_id = ctx.User.Id.ToString();
            } else
            {
                disc_user_id = target.Id.ToString();
            }
            //Console.WriteLine(disc_user_id);
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
            command.CommandText = @"SELECT user_id FROM playerregister WHERE discord_id = '" + disc_user_id + "'; ";
            //command.Parameters.AddWithValue("@user", disc_user_id);

            MySqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ce_user_id = reader.GetInt64(0).ToString();
                }
            }
            //Console.WriteLine(ce_user_id);
            HttpClient cartelClient = new HttpClient();
            FormUrlEncodedContent encoded = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "id", ce_user_id },
                { "type", "Advanced" },
                { "key", Secrets.CARTEL_API }
            });

            string queryString = await encoded.ReadAsStringAsync();

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, $"https://cartelempire.online/api/user?{queryString}");

            HttpResponseMessage resp = await cartelClient.SendAsync(req);

            string contents = await resp.Content.ReadAsStringAsync();

            User? userInfo = JsonSerializer.Deserialize<User>(contents);

            string hospOutput = string.Empty;
            string jailOutput = string.Empty;
            DateTime utcNow = DateTime.Now.ToUniversalTime();
            if (userInfo?.hospitalRelease > 0)
            {
                DateTime hospRelease = DateTimeOffset.FromUnixTimeSeconds((long)(userInfo?.hospitalRelease)).UtcDateTime;
                TimeSpan hospTime = hospRelease - utcNow;
                hospOutput = hospTime.ToString(@"hh\:mm\:ss");
            }

            if (userInfo?.jailRelease > 0)
            {
                DateTime jailRelease = DateTimeOffset.FromUnixTimeSeconds((long)(userInfo?.jailRelease)).UtcDateTime;
                TimeSpan jailTime = jailRelease - utcNow;
                jailOutput = jailTime.ToString(@"hh\:mm\:ss");
            }

            var totalYears = Math.Truncate((float)userInfo?.age / 365);
            var totalMonths = Math.Truncate(((float)userInfo?.age % 365) / 30);
            var remainingDays = Math.Truncate(((float)userInfo?.age % 365) % 30);

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

            embed.Title = userInfo?.name;
            //embed.Timestamp = new DiscordTimestamp(DateTime.Today);
            //embed.Footer = new EmbedFooter() { Text = DateTime.Today.ToShortDateString() + " || Inspirational quotes provided by ZenQuotes API" };
            //embed.Image = new EmbedMedia() { Url = "Media URL", Width = 150, Height = 150 }; //valid for thumb and video
            //embed.Provider = new EmbedProvider() { Name = "Provider Name", Url = "Provider Url" };
            embed.Url = "https://cartelempire.online/User/" + ce_user_id;
            embed.Color = new DiscordColor(44, 128, 106); //alpha will be ignored, you can use any RGB color
            //embed.Author = new EmbedAuthor() { Name = target_name, Url = "https://www.torn.com/profiles.php?XID=" + torn_id };
            DateTime active = DateTimeOffset.FromUnixTimeSeconds((long)userInfo?.lastActive).DateTime;
            TimeSpan activeAgo = DateTime.Now.Subtract(active);
            embed.Description = userInfo?.userType + " - Level " + userInfo?.level + Environment.NewLine + totalYears + " years, " + totalMonths + " months, " + remainingDays + " days old." + Environment.NewLine + "Last Online " + active + " (" + activeAgo.Minutes + " minutes ago)";

            /*switch (profileOutput["status"]["state"].ToString())
            {
                case "Okay":
                    embed.Color = DiscordColor.Green;
                    displaystatus = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:") + " " + profileOutput["status"]["description"] + " " + status_details;
                    break;
                case "Hospital":
                    embed.Color = DiscordColor.Red;
                    displaystatus = DiscordEmoji.FromName(ctx.Client, ":hospital:") + " " + profileOutput["status"]["description"] + " " + status_details;
                    break;
                case "Traveling":
                    embed.Color = DiscordColor.Blue;
                    displaystatus = DiscordEmoji.FromName(ctx.Client, ":airplane_arriving:") + " " + profileOutput["status"]["description"] + " " + status_details;
                    break;
                case "Abroad":
                    embed.Color = DiscordColor.Blue;
                    displaystatus = DiscordEmoji.FromName(ctx.Client, ":airplane:") + " " + profileOutput["status"]["description"] + " " + status_details;
                    break;
                case "Jail":
                    embed.Color = DiscordColor.Yellow;
                    displaystatus = DiscordEmoji.FromName(ctx.Client, ":scales:") + " " + profileOutput["status"]["description"] + " " + status_details;
                    break;
            }*/
            embed.AddField(DiscordEmoji.FromName(ctx.Client, ":blue_heart:") + " Status: " + userInfo?.currentLife + "/" + userInfo?.maxLife, userInfo?.status, true);

            /*try
            {
                embed.AddField(DiscordEmoji.FromName(ctx.Client, ":construction_worker:") + " Employment", profileOutput["job"]["position"] + " at [" + profileOutput["job"]["company_name"].ToString() + "](https://www.torn.com/joblist.php#/p=corpinfo&ID=" + profileOutput["job"]["company_id"].ToString() + ")", true);
            }
            catch
            {
                embed.AddField("Employment", "Unemployed", true);
            }*/

            if (userInfo.cartelId.HasValue)
            {
                FormUrlEncodedContent cartel_encoded = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "id", userInfo?.cartelId.ToString() },
                    { "type", "Basic" },
                    { "key", Secrets.CARTEL_API }
                });

                string cartel_query = await cartel_encoded.ReadAsStringAsync();
                HttpRequestMessage cartel_req = new HttpRequestMessage(HttpMethod.Get, $"https://cartelempire.online/api/cartel?{cartel_query}");

                HttpResponseMessage cartel_resp = await cartelClient.SendAsync(cartel_req);

                string cartel_contents = await cartel_resp.Content.ReadAsStringAsync();

                Cartel? cartelInfo = JsonSerializer.Deserialize<Cartel>(cartel_contents);

                embed.AddField(DiscordEmoji.FromName(ctx.Client, ":crossed_swords:") + " Cartel", "[" + cartelInfo?.name + "](https://cartelempire.online/cartel/" + cartelInfo?.id + ")", true);
            }
            else
            {
                embed.AddField("Cartel", "None", true);
            }

            /*try
            {
                embed.AddField(DiscordEmoji.FromName(ctx.Client, ":heart:") + " Marriage", "Married to [" + profileOutput["married"]["spouse_name"] + "](http://www.torn.com/profiles.php?XID=" + profileOutput["married"]["spouse_id"] + ") for " + marriedYears + " years, " + marriedMonths + " months, " + marriedDays + " days.", true);
            }
            catch
            {
                embed.AddField(DiscordEmoji.FromName(ctx.Client, ":broken_heart:") + " Marriage", "Single", true);
            }*/

            embed.AddField(DiscordEmoji.FromName(ctx.Client, ":bar_chart:") + " Social Statistics", "**Friends**: " + userInfo?.friendCount + Environment.NewLine + "**Enemies**: " + userInfo?.enemyCount, true);

            embed.AddField(DiscordEmoji.FromName(ctx.Client, ":speech_balloon:") + " Battle Statistics", "**Attacks Won**: " + userInfo?.attacksWon + Environment.NewLine + "**Defends Lost**: " + userInfo?.defendsLost, true);
            

            //lblReputation.Content = userInfo?.reputation;
            //lblJail.Content = jailOutput;
            //lblHospital.Content = hospOutput;
            await ctx.Channel.SendMessageAsync(embed);
        }
    }
}
