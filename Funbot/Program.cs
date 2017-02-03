using System;
using System.IO;
using System.Collections.Generic;
using DSLib.DiscordCommands;
using System.Timers;
using System.Threading.Tasks;

namespace Funbot
{
    class Program
    {
        static string[] gamesList = {"chercher quoi faire"};

        private static Timer gameTimer = new Timer(10 * 60000);

        static void Main(string[] args)
        {
            Bot bot = Bot.botInstance;
            ImageGenerator img = new ImageGenerator();
            string input = "as";

            bot.commandService.AddCommands(typeof(Questions), null);
            bot.commandService.AddCommands(typeof(ImageGenerator), img);
            bot.commandService.AddCommands(typeof(Program), null);
            bot.commandService.AddCommands(typeof(ServerManagement), null);

            bot.DiscordClient.Ready += DiscorClient_Ready;

            LoadGamesName("gamelist.txt");

            gameTimer.AutoReset = true;
            gameTimer.Elapsed += GameTimer_Elapsed;

            Console.WriteLine("Connexion...");
            Console.CursorTop--;
            bot.Connect();

            gameTimer.Start();

            while (input != "")
            {
                input = Console.ReadLine();
                if(input != "")
                {
                    try
                    {
                        bot.SetGame(input);
                    }
                    catch(Exception e)
                    {
                        WriteError("Tentative de changer le jeu échouée: " + e.Message);
                    }
                }
            }

            bot.Disconnect();
            Console.WriteLine("Fun Bot déconnecté");
        }

        private static void LoadGamesName(string filename)
        {
            try
            {
                StreamReader reader = new StreamReader(filename);
                List<string> gamesRead = new List<string>();

                while (!reader.EndOfStream)
                {
                    gamesRead.Add(reader.ReadLine());
                }

                gamesList = gamesRead.ToArray();
            }
            catch (FileNotFoundException)
            {
                Program.WriteError("Le fichier de nom de jeux n'a pas été trouvé");
            }
        }

        private static void GameTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Bot.botInstance.DiscordClient.SetGame(gamesList[Bot.rand.Next(gamesList.Length)]);
        }

        private static void DiscorClient_Ready(object sender, EventArgs e)
        {
            Bot.botInstance.DiscordClient.SetGame(gamesList[Bot.rand.Next(gamesList.Length)]);
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
