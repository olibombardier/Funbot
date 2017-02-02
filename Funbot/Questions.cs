using System;
using System.Threading;
using System.Collections.Generic;

using Discord;
using Discord.Commands;

namespace Funbot
{
    public class Questions
    {
        static readonly string[] normalAnswer = { "Oui, sans aucun doutes!", "Oui, probablement", "Oui", "Oui", "Non, c'est impossible", "non", "non", "Non, c'est peu probable" };
        static readonly string[] unsureAnswer = { "Tout dépend de toi", "Ça dépend...", "Je n'ai pas de réponse décisive... désolé", "Plusieurs possibilités sont probable..." };

        [Command("question", "Posez un question se répondant par oui ou non", "qu")]
        [Parameter("question", ParameterType.Optional)]
        static void Question(CommandEventArgs args)
        {
            string question = args.GetArg("question");

            Thread.Sleep(500);
            args.Channel.SendIsTyping();

            Thread.Sleep(Math.Max(1000, Math.Min(3700, question.Length * 80)));

            if (Bot.rand.Next(75) == 0)
            {
                switch (Bot.rand.Next(11))
                {
                    case 0:
                        args.Channel.SendMessage("Hahahahahahhaha!");
                        Thread.Sleep(1200);
                        args.Channel.SendMessage("non.");
                        break;

                    case 1:
                        args.Channel.SendMessage("Sérieusement?");
                        break;

                    case 2:
                        args.Channel.SendMessage("Selon la position de la lune, oui");
                        break;

                    case 3:
                        args.Channel.SendMessage("D'après l'alignement de Jupitère et Mars, non");
                        break;

                    case 4:
                        args.Channel.SendMessage("Bien sur! Sans aucun doute! Absolument!");
                        break;

                    case 5:
                        args.Channel.SendMessage("Humm... technicaly... Naaa!");
                        break;

                    case 6:
                        args.Channel.SendMessage("À moins d'un miracle, non");
                        break;

                    case 7:
                        args.Channel.SendMessage("Tu devrais demender à ta mère");
                        break;

                    case 8:
                        args.Channel.SendMessage("Oui");
                        Thread.Sleep(500);
                        args.User.PrivateChannel.SendMessage("J'ai menti, la vrai réponse est non");
                        break;

                    case 9:
                        args.Channel.SendMessage("Je ne peux divulguer cette information! Désolé!");
                        break;

                    case 10:
                        if (Bot.rand.Next(1) == 0)
                            args.Channel.SendMessage("Non évidement!");
                        else
                            args.Channel.SendMessage("Oui évidement!");
                        args.Channel.SendIsTyping();
                        Thread.Sleep(750);

                        args.Channel.SendMessage("Come on!");
                        args.Channel.SendIsTyping();
                        Thread.Sleep(400);

                        args.Channel.SendMessage("Pfff!");
                        args.Channel.SendIsTyping();
                        Thread.Sleep(800);

                        args.Channel.SendMessage("Quelle question!");
                        args.Channel.SendIsTyping();
                        Thread.Sleep(1000);

                        args.Channel.SendMessage("Réfléchit un peu!");
                        break;
                }
            }
            else
            {
                int answer = Bot.rand.Next(normalAnswer.Length + 1);
                if (answer != normalAnswer.Length)
                {
                    args.Channel.SendMessage(normalAnswer[answer]);
                }
                else
                {
                    answer = Bot.rand.Next(unsureAnswer.Length);
                    args.Channel.SendMessage(unsureAnswer[answer]);
                }
            }


        }

        [Command("who", "Posez une question dont la réponse devrait être une personne", "qui")]
        [Parameter("question", ParameterType.Required)]
        static void Who(CommandEventArgs args)
        {
            args.Channel.SendIsTyping();

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
            args.Channel.SendMessage(text);
        }

        [Command("thanks", "", "merci", "thx")]
        [Hidden]
        static void Thanks(CommandEventArgs args)
        {
            switch (Bot.rand.Next(2))
            {
                case 0:
                    args.Channel.SendMessage("Pas de problème");
                    break;

                case 1:
                    args.Channel.SendMessage("Ça m'a fait plaisir");
                    break;
            }
            
        }
    }
}
