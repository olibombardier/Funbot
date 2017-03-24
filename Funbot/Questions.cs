using System;
using System.Threading;
using System.Collections.Generic;

using Discord;
using DSLib.DiscordCommands;
using System.Threading.Tasks;

namespace Funbot
{
    public class Questions
    {
        static readonly string[] normalAnswer = { "Oui, sans aucun doutes!", "Oui, probablement", "Oui", "Oui", "Non, c'est impossible", "non", "non", "Non, c'est peu probable" };
        static readonly string[] unsureAnswer = { "Tout dépend de toi", "Ça dépend...", "Je n'ai pas de réponse décisive... désolé", "Plusieurs possibilités sont probable..." };

        [Command("question", "qu")]
        [CommandHelp("Posez un question se répondant par oui ou non", "")]
        [CommandParam(0, "question", true)]
        static async Task Question(CommandEventArgs args)
        {
            string question = args.GetArg("question");
            
            await args.Channel.SendIsTyping();

            Thread.Sleep(Math.Max(1000, Math.Min(3700, question.Length * 80)));

            if (Bot.rand.Next(75) == 0)
            {
                switch (Bot.rand.Next(11))
                {
                    case 0:
                        await args.Channel.SendMessage("Hahahahahahhaha!");
                        Thread.Sleep(1200);
                        await args.Channel.SendMessage("non.");
                        break;

                    case 1:
                        await args.Channel.SendMessage("Sérieusement?");
                        break;

                    case 2:
                        await args.Channel.SendMessage("Selon la position de la lune, oui");
                        break;

                    case 3:
                        await args.Channel.SendMessage("D'après l'alignement de Jupitère et Mars, non");
                        break;

                    case 4:
                        await args.Channel.SendMessage("Bien sur! Sans aucun doute! Absolument!");
                        break;

                    case 5:
                        await args.Channel.SendMessage("Humm... technicaly... Naaa!");
                        break;

                    case 6:
                        await args.Channel.SendMessage("À moins d'un miracle, non");
                        break;

                    case 7:
                        await args.Channel.SendMessage("Tu devrais demender à ta mère");
                        break;

                    case 8:
                        await args.Channel.SendMessage("Oui");
                        await Task.Delay(500);
                        await args.User.PrivateChannel.SendMessage("J'ai menti, la vrai réponse est non");
                        break;

                    case 9:
                        await args.Channel.SendMessage("Je ne peux divulguer cette information! Désolé!");
                        break;

                    case 10:
                        if (Bot.rand.Next(1) == 0)
                            await args.Channel.SendMessage("Non évidement!");
                        else
                            await args.Channel.SendMessage("Oui évidement!");
                        await args.Channel.SendIsTyping();
                        await Task.Delay(750);

                        await args.Channel.SendMessage("Come on!");
                        await args.Channel.SendIsTyping();
                        await Task.Delay(400);

                        await args.Channel.SendMessage("Pfff!");
                        await args.Channel.SendIsTyping();
                        await Task.Delay(800);

                        await args.Channel.SendMessage("Quelle question!");
                        await args.Channel.SendIsTyping();
                        await Task.Delay(1000);

                        await args.Channel.SendMessage("Réfléchit un peu!");
                        break;
                }
            }
            else
            {
                int answer = Bot.rand.Next(normalAnswer.Length + 1);
                if (answer != normalAnswer.Length)
                {
                    await args.Channel.SendMessage(normalAnswer[answer]);
                }
                else
                {
                    answer = Bot.rand.Next(unsureAnswer.Length);
                    await args.Channel.SendMessage(unsureAnswer[answer]);
                }
            }


        }

        [Command("who", "qui")]
        [CommandHelp("Posez une question dont la réponse devrait être une personne","")]
        [CommandParam(0, "question", true)]
        static async Task Who(CommandEventArgs args)
        {
            await args.Channel.SendIsTyping();

            List<User> connectedUsers = new List<User>();
            foreach (User u in args.Server.Users)
            {
                if (u.Status == UserStatus.Online)
                {
                    connectedUsers.Add(u);
                }
            }

            if (connectedUsers.Count <= 4)
            {
                foreach (User u in args.Server.Users)
                {
                    if (u.Status == UserStatus.Idle)
                    {
                        connectedUsers.Add(u);
                    }
                }
            }

            string text = connectedUsers[Bot.rand.Next(connectedUsers.Count)].Name;
            await args.Channel.SendMessage(text);
        }

        [Command("thanks", true, "merci", "thx", "mersi")]
        static async Task Thanks(CommandEventArgs args)
        {
            switch (Bot.rand.Next(2))
            {
                case 0:
                    await args.Channel.SendMessage("Pas de problème");
                    break;

                case 1:
                    await args.Channel.SendMessage("Ça m'a fait plaisir");
                    break;
            }
            
        }

        [Command("dice", "rand", "dé", "random")]
        [CommandParam(0, "max", optional: true)]
        [CommandParam(1, "min", optional: true)]
        [CommandHelp("Affiche un nombre aléatoire entre min et max (par défaut 1 et 6)", "")]
        static async Task Dice(CommandEventArgs args)
        {
            string maxString = args.GetArg("max");
            string minString = args.GetArg("min");

            int max = 6;
            int min = 1;

            if (maxString != null)
            {
                if (! int.TryParse(maxString, out max))
                {
                    throw new InvalidCommandArgumentException("Le maximum doit être un entier valide");
                }
            }

            if (minString != null)
            {
                if (!int.TryParse(minString, out min))
                {
                    throw new InvalidCommandArgumentException("Le minimum doit être un entier valide");
                }
            }

            if (max < min)
            {
                int temp = max;
                max = min;
                min = temp;
            }

            await args.Channel.SendMessage(Bot.rand.Next(min, max).ToString());
        }

        [Command("choose", "choisit")]
        [CommandParam(0, "valeurs", true, false)]
        [CommandHelp("Choisit un élément aléatoire dans un liste", "Choisit un élément aléatoire dans un liste, les éléments doivent être séparés par une virgule")]
        static async Task Choose(CommandEventArgs args)
        {
            string[] values = args.GetArg("valeurs").Split(new char[] { ',' });

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = values[i].Trim();
            }

            await args.Channel.SendMessage(values[Bot.rand.Next(values.Length)]);
        }
    }
}
