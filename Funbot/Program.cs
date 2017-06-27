using System;
using System.IO;
using System.Collections.Generic;
using DSLib.DiscordCommands;
using System.Timers;
using System.Threading.Tasks;
using System.Linq;
using Discord;
using System.Text;

namespace Funbot
{
    class Program
    {
        static string[] gamesList = {"chercher quoi faire"};
        static string[] roastsList = { "{0}... {0}... désolé, j'ai pas d'idée de roast..." };

        public const string gameFileName = "gamelist.txt";
        public const string roastFileName = "roastlist.txt";

        private static Timer gameTimer = new Timer(30 * 60000);

        static void Main(string[] args)
        {
            Console.Title = "Funbot";

            Bot bot = Bot.botInstance;
            ImageGenerator img = new ImageGenerator();
            BotDebug.InitDebug();

            BotDebug.GamesChanged += BotDebug_GamesChangedEvent;
            BotDebug.RoastsChanged += BotDebug_RoastsChangedEvent;

            BotDebug.Log("Ajout des commandes", "Init");
            bot.commandService.AddCommands(typeof(BotDebug), null);
            bot.commandService.AddCommands(typeof(Questions), null);
            bot.commandService.AddCommands(typeof(ImageGenerator), img);
            bot.commandService.AddCommands(typeof(Program), null);
            bot.commandService.AddCommands(typeof(ServerManagement), null);

            bot.DiscordClient.Ready += DiscorClient_Ready;
            bot.commandService.CommandException += OnCommandException;


            BotDebug.Log("Lecture des jeux", "Init");
            gamesList = LoadLines(gameFileName);
            
            BotDebug.Log("Lecture des roasts");
            roastsList = LoadLines(roastFileName);

            gameTimer.AutoReset = true;
            gameTimer.Elapsed += GameTimer_Elapsed;

            WriteLine("Connexion...");
            Console.CursorTop--;
            bot.Connect();

            gameTimer.Start();

            Console.ReadKey();

            bot.Disconnect();
            BotDebug.Log("Fun Bot déconnecté");

            BotDebug.StopDebug();
            WriteLine("Fin du programme");
        }

        static void BotDebug_RoastsChangedEvent()
        {
            BotDebug.Log("Lecture des nouveau roast roasts");
            roastsList = LoadLines(roastFileName);
        }

        static void BotDebug_GamesChangedEvent()
        {
            BotDebug.Log("Lecture des nouveau jeux");
            gamesList = LoadLines(gameFileName);
        }

        private static void OnCommandException(CommandExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            CommandEventArgs command = e.Command;

            DateTime now = DateTime.Now;
            string nowString = "[" + now.ToShortTimeString() + "]";

            StringBuilder messageBuilder = new StringBuilder();

            messageBuilder.Append(
                nowString +
                "An error occured in the command \"" + 
                command.Message.RawText +
                "\" by " + 
                command.User.Name + ": ");

            messageBuilder.Append(e.Exception.ToString());
            messageBuilder.Append("In channel " + command.Channel.Name);
            messageBuilder.Append("");

            BotDebug.Log(messageBuilder.ToString(), "Error", ConsoleColor.Red);

            command.Channel.SendMessage("```Une erreur s'est produite à l'exécution de la commande.```");
        }

        private static string[] LoadLines(string filename)
        {
            List<string> linesRead = new List<string>();

            try
            {
                StreamReader reader = new StreamReader(filename);

                while (!reader.EndOfStream)
                {
                    linesRead.Add(reader.ReadLine());
                }
                
                reader.Close();
            }
            catch (FileNotFoundException)
            {
                BotDebug.Log("Le fichier \"" + filename + "\" n'a pas été trouvé", "FileError", ConsoleColor.Red);
            }

            return linesRead.ToArray();
        }

        private static void SaveLines(string filename, string[] lines)
        {
            StreamWriter writer;
            try
            {
                using (writer = new StreamWriter(filename, false))
                {
                    foreach (string name in lines)
                        {
                            if (name != "")
                            {
                                writer.WriteLine(name);
                            }
                        }
                }
            }
            catch (FileNotFoundException)
            {
                BotDebug.Log("Le fichier \"" + filename + "\" n'a pas été trouvé", "FileError", ConsoleColor.Red);
            }
        }

        private static void SaveLine(string filename, string line)
        {
            StreamWriter writer;
            try
            {
                using (writer = new StreamWriter(filename, true))
                {
                    
                    writer.WriteLine(line);
                }
            }
            catch (FileNotFoundException)
            {
                BotDebug.Log("Le fichier \"" + filename + "\" n'a pas été trouvé", "FileError", ConsoleColor.Red);
            }
        }

        private static void GameTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Bot.botInstance.DiscordClient.SetGame(gamesList[Bot.rand.Next(gamesList.Length)]);
        }

        private static void DiscorClient_Ready(object sender, EventArgs e)
        {
            WriteLine("Le bot est prêt à l'utilisation!");
            Bot.botInstance.DiscordClient.SetGame(gamesList[Bot.rand.Next(gamesList.Length)]);
        }

        [Command("addgame")]
        [CommandHelp("Ajoute un jeu auquel le bot peux jouer", "")]
        [CommandParam(0, "gamename", true)]
        static async Task AddGame(CommandEventArgs args)
        {
            string gamename = args.GetArg("gamename");

            if (!gamesList.Contains(gamename))
            {
                string[] newGameList = new string[gamesList.Length + 1];
                gamesList.CopyTo(newGameList, 0);
                newGameList[gamesList.Length] = gamename;
                gamesList = newGameList;

                SaveLine(gameFileName, gamename);
                await args.Channel.SendMessage("Le jeu " + gamename + " à été ajouté.");
                BotDebug.Log("A game has been added (" + gamename + ")");
            }
        }

        [Command("addroast")]
        [CommandHelp("Ajoute un roast au bot", "Usage: !addroast [roast]\n {0} sera remplacé pas le nom de la personne roastée")]
        [CommandParam(0, "roast", true)]
        static async Task AddRoast(CommandEventArgs args)
        {
            if (args.Server.Id == 210360089318522880ul)
            {
                await args.Channel.SendMessage("Commande désactivée dans ce sereur pour en respecter le climat pédagogique ¯\\_(ツ)_/¯");
                return;
            }

            string roast = args.GetArg("roast");

            if (!gamesList.Contains(roast))
            {
                string[] newRoastList = new string[roastsList.Length + 1];
                roastsList.CopyTo(newRoastList, 0);
                newRoastList[roastsList.Length] = roast;
                roastsList = newRoastList;

                SaveLine(roastFileName, roast);
                await args.Channel.SendMessage(String.Format("Le roast " + roast + " a été ajouté", "(...)"));
                BotDebug.Log("A roast has been added (" + roast + ")");
            }
        }

        [Command("roast")]
        [CommandHelp("Roast une personne, spécifiez la personne avec une mention ou par un nom quelconque", "")]
        [CommandParam(0, "cible", true, true)]
        static async Task Roast(CommandEventArgs args)
        {
            if (args.Server.Id == 210360089318522880ul)
            {
                await args.Channel.SendMessage("Commande désactivée dans ce sereur pour en respecter le climat pédagogique ¯\\_(ツ)_/¯");
                return;
            }

            string target = args.GetArg("cible");
            string targetName = "qqn";
            
            if(target == null)
            {
                if (args.Channel.IsPrivate)
                {
                    targetName = args.User.Name;
                }
                else
                {
                    User[] users = args.Server.Users.Where((u) => (u.Status != UserStatus.Offline)).ToArray();
                    targetName = Bot.getUserName(users[Bot.rand.Next(users.Length)]);
                }
            }
            else if(target.ToLower() == "me")
            {
                targetName = args.User.Name;
            }
            else
            {
                ulong id = 0;
                if(Bot.TryGetIdFromMention(target, ref id))
                {
                    targetName = Bot.getUserName(args.Server.GetUser(id));
                }
                else
                {
                    targetName = target;
                }
            }
            int chosenRoast = Bot.rand.Next(roastsList.Length);
            string roast = String.Format(roastsList[chosenRoast], targetName);
            BotDebug.OnRoast(chosenRoast);
            await args.Channel.SendMessage(roast);
        }

        [Command("stats", "stat")]
        [CommandHelp("Affiche certaines statistiques du bot", "")]
        static async Task Stats(CommandEventArgs args)
        {
            StringBuilder builder = new StringBuilder("```");

            builder.Append("Nombre de jeux: ");
            builder.AppendLine(gamesList.Length.ToString());

            builder.Append("Nombre de roasts: ");
            builder.AppendLine(roastsList.Length.ToString());

            builder.Append("```");
            await args.Channel.SendMessage(builder.ToString());
        }

        [Command("easteregg")]
        static async Task EasterEgg(CommandEventArgs args)
        {
            await args.User.PrivateChannel.SendMessage("Wow! T'as trouvé un easter egg!!!\nC'est dont ben cool!\nUn easter egg!\nUN EASTER EGG!!!!!\nWOOOOOOOOOOOOOOOOOOOOOOOOOW!!!!!");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("{0} à trouvé l'easter egg", args.User.Name);
            Console.ResetColor();
        }

        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteLine(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void Write(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }
    }
}
