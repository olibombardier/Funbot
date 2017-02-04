using System;
using System.IO;
using System.Collections.Generic;
using DSLib.DiscordCommands;
using System.Timers;
using System.Threading.Tasks;
using System.Linq;
using Discord;

namespace Funbot
{
    class Program
    {
        static string[] gamesList = {"chercher quoi faire"};
        static string[] roastsList = { "{0}... {0}... désolé, j'ai pas d'idée de roast..." };

        const string gameFileName = "gamelist.txt";
        const string roastFileName = "roastlist.txt";

        private static Timer gameTimer = new Timer(30 * 60000);

        static void Main(string[] args)
        {
            Bot bot = Bot.botInstance;
            ImageGenerator img = new ImageGenerator();
            string input = "as";

            WriteLine("Ajout des commandes");
            bot.commandService.AddCommands(typeof(Questions), null);
            bot.commandService.AddCommands(typeof(ImageGenerator), img);
            bot.commandService.AddCommands(typeof(Program), null);
            bot.commandService.AddCommands(typeof(ServerManagement), null);

            bot.DiscordClient.Ready += DiscorClient_Ready;

            WriteLine("Lecture des jeux");
            gamesList = LoadLines(gameFileName);
            
            WriteLine("Lecture des roasts");
            roastsList = LoadLines(roastFileName);

            gameTimer.AutoReset = true;
            gameTimer.Elapsed += GameTimer_Elapsed;

            WriteLine("Connexion...");
            Console.CursorTop--;
            bot.Connect();

            gameTimer.Start();

            Console.ReadKey();

            bot.Disconnect();
            Console.WriteLine("Fun Bot déconnecté");

            WriteLine("Fin du programme");
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
                WriteError("Le fichier \"" + filename + "\" n'a pas été trouvé");
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
                WriteError("Le fichier \"" + filename + "\" n'a pas été trouvé");
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
                WriteError("Le fichier \"" + filename + "\" n'a pas été trouvé");
            }
        }

        private static void GameTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Bot.botInstance.DiscordClient.SetGame(gamesList[Bot.rand.Next(gamesList.Length)]);
        }

        private static void DiscorClient_Ready(object sender, EventArgs e)
        {
            Console.WriteLine("Le bot est prêt à l'utilisation!");
            Bot.botInstance.DiscordClient.SetGame(gamesList[Bot.rand.Next(gamesList.Length)]);
        }

        [Command("addgame")]
        [CommandHelp("Ajoute un jeu auquel le bot peux jouer", "")]
        [CommandParam(0, "gamename", true)]
        static async Task AddGame(CommandEventArgs args)
        {
            string gamename = args.GetArg("gamename");
            
            WriteLine(args.User.Name + " veux ajouter le jeu " + gamename);

            if (!gamesList.Contains(gamename))
            {
                string[] newGameList = new string[gamesList.Length + 1];
                gamesList.CopyTo(newGameList, 0);
                newGameList[gamesList.Length] = gamename;
                gamesList = newGameList;

                SaveLine(gameFileName, gamename);
            }
        }

        [Command("addroast")]
        [CommandHelp("Ajoute un roast au bot", "")]
        [CommandParam(0, "roast", true)]
        static async Task AddRoast(CommandEventArgs args)
        {
            string roast = args.GetArg("roast");

            WriteLine("Roast ajouté");

            if (!gamesList.Contains(roast))
            {
                string[] newRoastList = new string[roastsList.Length + 1];
                roastsList.CopyTo(newRoastList, 0);
                newRoastList[roastsList.Length] = roast;
                roastsList = newRoastList;

                SaveLine(roastFileName, roast);
                await args.Channel.SendMessage(String.Format("Le roast " + roast + " a été ajouté", "(...)"));
            }
        }

        [Command("roast")]
        [CommandHelp("Roast une personne, spécifiez la personne avec une mention", "")]
        [CommandParam(0, "target", true, true)]
        static async Task Roast(CommandEventArgs args)
        {
            string target = args.GetArg("target");
            string targetName = "qqn";

            if(target == null)
            {
                User[] users = args.Server.Users.Where((u) => (u.Status != UserStatus.Offline)).ToArray();
                targetName = users[Bot.rand.Next(users.Length)].Name;
            }
            else
            {
                ulong id = 0;
                if(Bot.TryGetIdFromMention(target, ref id))
                {
                    targetName = args.Server.GetUser(id).Name;
                }
                else
                {
                    targetName = target;
                }
            }

            string roast = String.Format(roastsList[Bot.rand.Next(roastsList.Length)], targetName);
            await args.Channel.SendMessage(roast);
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
