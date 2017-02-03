using System;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using System.Timers;

using Discord;
using DSLib.DiscordCommands;
using System.Collections.Generic;
using System.Text;

namespace Funbot
{
    class Bot
    {
        private DiscordClient client;
        public DiscordClient DiscordClient
        {
            get { return client; }
        }

        private List<CommandHelp> commandHelp = new List<CommandHelp>();

        private static readonly string[] answerHello = { "Salut!", "Hello!", "Coucou!", "Hey!", "Hello!", "Yo!", "Hello! :smile:" };
        private static readonly string[] answerBye = { "Bye!", "Au revoir!", "À la procahine!", "Bye bye!" };

        public static readonly Random rand = new Random((int)DateTime.Now.Ticks);

        public static Bot botInstance { get; private set; }

        public CommandService commandService { get; private set; }

        static Bot()
        {
            botInstance = new Bot();

            botInstance.commandService.AddCommands(typeof(Bot), botInstance);
        }

        private Bot()
        {
            client = new DiscordClient(x => { x.LogLevel = LogSeverity.Info; });
            commandService = client.UsingCommands();
            commandService.CommandPrefixes.Add("!", null);
            commandService.CommandPrefixes.Add("", (args) => args.Channel.IsPrivate);
        }

        public void Connect()
        {
            client.Connect("MjcwNjMyMTc2MjI2MTQwMTcx.C2KAJA.bbAC6ZdH_i-L7yBWhrQ8eyjxX0M", TokenType.Bot);
            Console.WriteLine("Fun Bot connecté");
        }

        public void Disconnect()
        {
            client.Disconnect();
        }

        [Command("git", "Affiche le lien vers la page github du bot")]
        static async Task Git(CommandEventArgs args)
        {
            await args.Channel.SendMessage("https://github.com/olibombardier/Funbot \nN'hésitez pas à contribuer!");
        }

        [Command("help", "Affiche la liste des commandes disponnible ainsi que leurs instructions", "aide")]
        public async Task Help(CommandEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            await args.User.PrivateChannel.SendIsTyping();

            builder.AppendLine("Fun Bot!");
            builder.AppendLine("Bot ayant pour utilité de ne pas être tant utile que ça.");
            builder.AppendLine("Creéé par Olivier Bombardier");
            builder.AppendLine();
            builder.AppendLine("\t\t-Commandes-");

            foreach (CommandHelp help in commandHelp)
            {
                builder.Append("!");
                builder.Append(help.CommandName);
                if (help.Aliases.Length > 0)
                {
                    builder.Append(" ( alias: ");
                    for (int i = 0; i < help.Aliases.Length; i++)
                    {
                        if (i != 0)
                        {
                            builder.Append(", ");
                        }

                        builder.Append(help.Aliases[i]);
                    }
                    builder.Append(" )");
                }
                builder.AppendLine();
                if (help.HelpText != "")
                {
                    builder.Append("\t-");
                    builder.AppendLine(help.HelpText);
                }
            }

            await args.User.PrivateChannel.SendMessage(builder.ToString());
        }

        [Command("hello", "Dites bonjour à Fun Bot", "salut", "bonjour", "yo", "allo", "bonsoir", "hey")]
        static async Task Hello(CommandEventArgs args)
        {
            await args.Channel.SendMessage(answerHello[rand.Next(answerHello.Length)]);
        }

        [Command("bye", "Dites au revoir à Fun Bot", "byebye")]
        [CommandParam(0, "bye", true, true)]
        static async Task Bye(CommandEventArgs args)
        {
            await args.Channel.SendMessage(answerBye[rand.Next(answerBye.Length)]);
        }

        public void SetGame(string gameName)
        {
            Program.WriteLine("Changement de jeu pour " + gameName, ConsoleColor.DarkMagenta);
            client.SetGame(gameName);
            Program.WriteLine("Le jeu à été changé pour " + client.CurrentGame.Name, ConsoleColor.DarkMagenta);
        }

        [Command("test")]
        [CommandParam(0, "message", true)]
        public async Task Log(CommandEventArgs args)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            Console.Write(args.User.Name + " (");
            Console.Write(args.User.Id);
            Console.Write(") - " + args.Channel.Name + ": ");
            Console.ResetColor();
            Console.WriteLine(args.GetArg("message"));
        }

        public static ulong GetIdFromMention(string Mention)
        {
            ulong result = 0;

            if (Mention[0] == '<' && Mention[1] == '@' && Mention.EndsWith(">"))
            {
                string idString = Mention.Substring(2, Mention.Length - 3);

                result = ulong.Parse(idString);
            }
            else
            {
                throw new ArgumentException("La mention n'est pas valide", "Mention");
            }
            return result;
        }

        public static bool TryGetIdFromMention(string Mention, ref ulong result)
        {
            bool noError = true;
            try
            {
                result = GetIdFromMention(Mention);
            }
            catch (Exception)
            {
                noError = false;
            }
            return noError;
        }
    }
}
