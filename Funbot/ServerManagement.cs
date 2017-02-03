using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using DSLib.DiscordCommands;

namespace Funbot
{
    static class ServerManagement
    {
        static readonly string[] colorRoles = { "red", "orange", "yellow", "green", "blue", "purple", "cyan" };

        [Command("couleur", "color")]
        [CommandHelp("Change ta couleur dans le serveur \"Bomb Power\".","Usage: !couleur [couleur]")]
        [CommandParam(0, "colorname")]
        static async Task Color(CommandEventArgs args)
        {
            User user = args.User;
            string colorName = args.GetArg("colorname").ToLower();

            if (args.Channel.IsPrivate)
            {
                await args.Channel.SendMessage("Impossible de changer la couleur dans une cenversation privée.");
            }
            else if (colorRoles.Contains(colorName))
            {
                await user.RemoveRoles(getUserColorRoles(user));

                await Task.Delay(750);

                foreach (Role r in args.Server.Roles)
                {
                    if (r.Name.ToLower() == colorName)
                    {
                        await Bot.botInstance.DiscordClient.GetServer(args.Server.Id).GetUser(user.Id).AddRoles(r);
                    }
                }
            }
            else if (colorName == "default")
            {
                await user.RemoveRoles(getUserColorRoles(user));
            }
            else
            {
                StringBuilder builder = new StringBuilder("Veuillez entrer une couleur valide (");
                for (int i = 0; i < colorRoles.Length; i++)
                {
                    if (i != 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(colorRoles[i]);
                }

                builder.Append(") ou 'default' pour avoir la couleur de base.");
                await args.Channel.SendMessage(builder.ToString());
            }
        }

        static Role[] getUserColorRoles(User user)
        {
            return user.Roles.Where(r => { return colorRoles.Contains(r.Name.ToLower()); }).ToArray();
        }
    }
}
