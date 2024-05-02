using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vidar
{
    internal class Lotto : BaseCommandModule
    {
        [Command("startlotto")]
        [Aliases("sl")]
        public async Task StartLottoCommand(CommandContext ctx)
        {
            await ctx.RespondAsync("The command to start a lotto should be running.");
        }
    }
}
