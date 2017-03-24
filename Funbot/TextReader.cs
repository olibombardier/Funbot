using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using DSLib.DiscordCommands;
using System.Collections.Generic;
using System.Text;

namespace Funbot
{
    static class TextReader
    {
        [Command("readchannel")]
        [CommandParam(0, "name", true)]
        public static async Task ReadChannel(CommandEventArgs args)
        {
        }
    }
}
