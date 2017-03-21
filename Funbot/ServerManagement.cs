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

        //[Command("addroom")]
        [CommandHelp("Ajoute une room","Usage: \n\t!addroom [name]\n\t!addroom [room] [public/secret]")]
        [CommandParam(0, "roomName")]
        [CommandParam(1, "privacy", optional:true)]
        static async Task AddRoomCmd(CommandEventArgs args)
        {
            bool isSecret;
            string privacyArg = args.GetArg("privacy");
            string roomName = args.GetArg("roomName");

            if (privacyArg == null || privacyArg == "public")
            {
                isSecret = false;
            }
            else if (privacyArg == "secret")
            {
                isSecret = true;
            }
            else
            {
                await args.Channel.SendMessage("L'argument \"privacy\" doit être soit \"public\" ou \"secret\"");
                return;
            }

            if (roomName == null)
            {
                await args.Channel.SendMessage("Vous devez spécifier un nom.");
            }
            else
            {
                Role botRole = null;
                Role everyoneRole = args.Server.EveryoneRole;

                ChannelPermissionOverrides botPermission = 
                    new ChannelPermissionOverrides(
                        readMessages: PermValue.Allow,
                        sendMessages: PermValue.Allow,
                        manageChannel: PermValue.Allow
                        );

                ChannelPermissionOverrides notAllowedPermission =
                    new ChannelPermissionOverrides(
                        readMessages: PermValue.Deny
                        );

                foreach(Role r in args.Server.Roles)
                {
                    if(r.Name == "Bot")
                    {
                        botRole = r;
                        break;
                    }
                }
                 
                if(botRole == null)
                {
                    throw new Exception("Le rôle de Bot n'a pas été trouvé");
                }

                Channel newChannel = await args.Server.CreateChannel(roomName, ChannelType.Text);
                await newChannel.AddPermissionsRule(botRole, botPermission);
                await newChannel.AddPermissionsRule(args.User, botPermission);

                BotDebug.Log("Le channel " + roomName + " à été créé par " + args.User.Name,
                        "ServerManagement", ConsoleColor.Yellow);
                
                if (isSecret)
                {
                    await newChannel.AddPermissionsRule(everyoneRole, notAllowedPermission);
                }

                await newChannel.SendMessage(args.User.Mention + ", votre nouveau salon à été créé, vous pouvez en modifier les permissions.\n" +
                    "Prenez notes que les administrateurs ont toujours accès à ce salon.");
            }
        }

        static Role[] getUserColorRoles(User user)
        {
            return user.Roles.Where(r => { return colorRoles.Contains(r.Name.ToLower()); }).ToArray();
        }
    }
}
