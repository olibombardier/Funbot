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

        public static readonly EnvironmentRandom rand = new EnvironmentRandom();

        public static Bot botInstance { get; private set; }

        public CommandService commandService { get; private set; }

        static Bot()
        {
            botInstance = new Bot();

            botInstance.commandService.AddHelpCommand();
            botInstance.commandService.AddCommands(typeof(Bot), botInstance);
        }

        private Bot()
        {
            client = new DiscordClient(
                x => { x.LogLevel = LogSeverity.Info;
                x.LogHandler = BotDebug.OnDiscordLog;
            });
            commandService = client.UsingCommands();
            commandService.CommandPrefixes.Add("!", null);
            commandService.CommandPrefixes.Add("fb!", null);
            commandService.CommandPrefixes.Add("funbot!", null);
            commandService.CommandPrefixes.Add("", (args) => args.Channel.IsPrivate ||
                                                             args.Channel.Id == 272031256282267658);
        }

        public void Connect()
        {
            string botToken = "";

            using (StreamReader reader = new StreamReader("FunBotToken.txt"))
            {
                botToken = reader.ReadLine();
                botToken.TrimEnd();
            }

            client.Connect(botToken, TokenType.Bot);
        }

        public void Disconnect()
        {
            client.Disconnect();
        }

        [Command("git")]
        [CommandHelp("Affiche le lien vers le dépot github du bot", "")]
        static async Task Git(CommandEventArgs args)
        {
            await args.Channel.SendMessage("https://github.com/olibombardier/Funbot \nN'hésitez pas à contribuer!");
        }

        [Command("hello", "salut", "bonjour", "yo", "allo", "bonsoir", "hey", "coucou", "hola")]
        [CommandHelp("Dites bonjour à Fun Bot", "")]
        static async Task Hello(CommandEventArgs args)
        {
            await args.Channel.SendMessage(answerHello[rand.Next(answerHello.Length)]);
        }

        [Command("bye", "byebye")]
        [CommandHelp("Dites au revoir à Fun Bot", "")]
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

        /// <summary>
        /// Get the name used by the user on the server
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string getUserName(User user)
        {
            string result = user.Nickname;

            Console.WriteLine("Nick {0}", result);

            if (result == null)
            {
                result = user.Name;
            }


            Console.WriteLine("result {0} \n", result);

            return result;
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

    class InvalidCommandArgumentException : Exception
    {
        public InvalidCommandArgumentException()
        {
        }

        public InvalidCommandArgumentException(string message)
            : base(message)
        {
        }

        public InvalidCommandArgumentException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
