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
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            string input = "as";

            bot.CreateCommandsFromClass(typeof(Questions));
            bot.CreateCommandsFromClass(typeof(ImageGenerator));
            bot.CreateCommandsFromClass(typeof(Program));

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
