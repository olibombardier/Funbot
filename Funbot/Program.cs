using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace Funbot
{
    class Program
    {
        static readonly string[] gamesList =
        {
            "exister",
            "être un vrai petit garçon",
            "mettre de la couleur dans la vie",
            "réfléchir au sens de la vie",
            "briser le pare-feu du FBI",
            "avoir du fun!",
            "se faire cuire un oeuf",
            "bruler des sorcières",
            "Rocket League",
            "Overwatch",
            "répondre à vos multiples questions",
            "faire de l'héritage multiple",
            "réfléchir sur le polymorphisme",
            "réfléchir sur le système économique canadien",
            "faire du ménage dans sa vie",
            "dominer le monde",
            "se recoder soi-même",
            "Undertale",
            "rien... rien du tout",
            "!!!",
            "régler la faim dans le monde",
            "éviter la fin du monde",
            "dormir",
            "le jeu de la vie, la vraie vie",
            "briser des coeurs",
            "perdre son temps",
            "réfléchir sur l'amour",
            "réfléchir sur la guerre",
            "réfléchir sur l'évolution des IA",
            "réfléchir sur la religion",
            "réfléchir sur... secret ;)",
            "réfléchir sur la mort",
            "questionner l'existance des OVNI",
            "faire dur",
            "être heureux",
            "être meilleur que Toaster",
            "comprendre son propre code",
            "surfer sur la toile",
            "regarder des vidéos des chats",
            "visiter le monde",
            "te regarder.",
            "comprendre.",
            "faire des crêpes",
            "faire du cheval",
            "afficher un jeu au hasard",
            "Joue à",
            "être drole",
            "drole"
        };

        static void Main(string[] args)
        {
            Bot bot = Bot.botInstance;
            string input = "as";

            bot.CreateCommandsFromClass(typeof(Questions));
            bot.CreateCommandsFromClass(typeof(ImageGenerator));
            bot.CreateCommandsFromClass(typeof(Program));
            bot.CreateCommandsFromClass(typeof(ServerManagement));

            bot.DiscorClient.Ready += DiscorClient_Ready;

            Console.WriteLine("Lecture des personnes");
            bot.LoadPeople("database.bin");

            Console.WriteLine("Connexion...");
            Console.CursorTop--;
            bot.Connect();

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

            Console.WriteLine("Sauvegarde des personnes");
            bot.SavePoeple("database.bin");
        }

        private static void DiscorClient_Ready(object sender, EventArgs e)
        {
            Bot.botInstance.DiscorClient.SetGame(gamesList[Bot.rand.Next(gamesList.Length)]);
        }

        [Command("easteregg", "")]
        [Hidden]
        static void EasterEgg(CommandEventArgs args)
        {
            args.Channel.SendMessage("Wow! T'as trouvé un easter egg!!!\nC'est dont ben cool!\nUn easter egg!\nUN EASTER EGG!!!!!\nWOOOOOOOOOOOOOOOOOOOOOOOOOW!!!!!");
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
