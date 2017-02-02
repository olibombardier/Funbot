using System;
using System.Reflection;
using System.IO;

using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Text;

namespace Funbot
{
    class Bot
    {
        private DiscordClient client;
        private List<CommandHelp> commandHelp = new List<CommandHelp>();

        private Dictionary<ulong, Person> users = new Dictionary<ulong, Person>();

        private static readonly string[] answerHello = { "Salut!", "Hello!", "Coucou!", "Hey!", "Hello!", "Yo!", "Hello! :smile:" };
        private static readonly string[] answerBye = { "Bye!", "Au revoir!", "À la procahine!", "Bye bye!" };

        public static readonly Random rand = new Random((int)DateTime.Now.Ticks);

        public Bot()
        {
            client = new DiscordClient(x => { x.LogLevel = LogSeverity.Info; });
            client.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
            });

            CreateCommandsFromObject(this);
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

        public void CreateCommandsFromClass(Type classType)
        {
            CommandService commandService = client.GetService<CommandService>();

            foreach (MethodInfo method in classType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                CommandAttribute command = method.GetCustomAttribute<CommandAttribute>();
                if (command != null)
                {
                    CommandHelp help = new CommandHelp();

                    CommandBuilder builder = commandService.CreateCommand(command.Name);
                    help.CommandName = command.Name;
                    help.HelpText = command.HelpText;

                    foreach (string alias in command.Aliases)
                    {
                        builder.Alias(alias);
                    }
                    help.Aliases = command.Aliases;

                    IEnumerable<ParameterAttribute> paramList = method.GetCustomAttributes<ParameterAttribute>();
                    foreach (ParameterAttribute param in paramList)
                    {
                        builder.Parameter(param.Name, param.Type);
                    }

                    if (method.IsStatic)
                    {
                        builder.Do((Action<CommandEventArgs>)Delegate.CreateDelegate(typeof(Action<CommandEventArgs>), method));
                        Console.WriteLine("Commend added: {0}.", command.Name);

                        if (method.GetCustomAttribute<HiddenAttribute>() == null)
                        {
                            commandHelp.Add(help);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("La comande \"" + command.Name + "\" ne peut pas être utilisée car elle n'est pas statique");
                        Console.ResetColor();
                    }
                }
            }
        }

        public void CreateCommandsFromObject(Type classType, object target)
        {
            CommandService commandService = client.GetService<CommandService>();

            foreach (MethodInfo method in classType.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                CommandAttribute command = method.GetCustomAttribute<CommandAttribute>();
                if (command != null)
                {
                    CommandHelp help = new CommandHelp();

                    CommandBuilder builder = commandService.CreateCommand(command.Name);
                    help.CommandName = command.Name;
                    help.HelpText = command.HelpText;

                    foreach (string alias in command.Aliases)
                    {
                        builder.Alias(alias);
                    }
                    help.Aliases = command.Aliases;

                    IEnumerable<ParameterAttribute> paramList = method.GetCustomAttributes<ParameterAttribute>();
                    foreach (ParameterAttribute param in paramList)
                    {
                        builder.Parameter(param.Name, param.Type);
                    }

                    if (method.IsStatic)
                    {
                        builder.Do((Action<CommandEventArgs>)Delegate.CreateDelegate(typeof(Action<CommandEventArgs>), method));
                        Console.WriteLine("Commend added: {0}.", command.Name);
                        if (method.GetCustomAttribute<HiddenAttribute>() == null)
                        {
                            commandHelp.Add(help);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Commend added: {0}.", command.Name);
                        builder.Do((Action<CommandEventArgs>)Delegate.CreateDelegate(typeof(Action<CommandEventArgs>), target, method));
                        if (method.GetCustomAttribute<HiddenAttribute>() == null)
                        {
                            commandHelp.Add(help);
                        }
                    }
                }
            }
        }

        public void LoadPeople(string filename)
        {
            if (File.Exists(filename))
            {
                try
                {
                    using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
                    {
                        int personCount = reader.ReadInt32();
                        for (int i = 0; i < personCount; i++)
                        {
                            Person newPerson = Person.Read(reader);
                            users.Add(newPerson.Id, newPerson);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Une erreur est survenue à la lecture des usagers: " + e.Message);
                    Console.WriteLine("Il est possible que la liste d'usagers soit corrompue ou imcomplète.");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Impossible de charger les usagers à partir du fichier \"" + filename + "\" car il n'existe pas!");
                Console.ResetColor();
            }
        }

        public void SavePoeple(string filename)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    writer.Write(users.Count);

                    foreach (KeyValuePair<ulong, Person> p in users)
                    {
                        p.Value.Save(writer);
                    }
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Une erreur est survenue à l'écriture lecture des usagers: " + e.Message);
                Console.WriteLine("Il est possible que la liste d'usagers soit corrompue ou imcomplète.");
                Console.ResetColor();
            }
        }

        public void SetUserMoney(ulong Id, long value)
        {
            if (!users.ContainsKey(Id))
            {
                users[Id] = new Person() { Id = Id, Money = 0 };
            }

            users[Id].Money = value;
        }

        public long GetUserMoney(ulong Id)
        {
            if (!users.ContainsKey(Id))
            {
                return 0;
            }
            else
            {
                return users[Id].Money;
            }
        }

        [Command("help", "Affiche la liste des commandes disponnible ainsi que leurs instructions", "aide")]
        public void Help(CommandEventArgs args)
        {
            StringBuilder builder = new StringBuilder();
            args.User.PrivateChannel.SendIsTyping();

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

            args.User.PrivateChannel.SendMessage(builder.ToString());
        }

        [Command("hello", "Dites bonjour à Fun Bot", "salut", "bonjour", "yo", "allo", "bonsoir", "hey")]
        static void Hello(CommandEventArgs args)
        {
            args.Channel.SendMessage(answerHello[rand.Next(answerHello.Length)]);
        }

        [Command("bye", "Dites au revoir à Fun Bot", "byebye")]
        [Parameter("target", ParameterType.Optional)]
        static void Bye(CommandEventArgs args)
        {
            args.Channel.SendMessage(answerBye[rand.Next(answerBye.Length)]);
        }

        [Command("money", "Affiche combien un utilisateur à d'argent")]
        [Parameter("user", ParameterType.Optional)]
        public void Money(CommandEventArgs args)
        {
            User user = null;
            string userArg = args.GetArg("user");
            if (args.GetArg("user") == "")
            {
                user = args.User;
            }
            else
            {
                if (userArg[0] == '<' && userArg[1] == '@' && userArg.EndsWith(">"))
                {
                    string idString = userArg.Substring(2, userArg.Length - 3);
                    ulong id;

                    if (ulong.TryParse(idString, out id))
                    {
                        user = args.Channel.GetUser(id);
                    }
                    else
                    {
                        args.Channel.SendMessage("Cette personnes à: " + GetUserMoney(id) + "$.");
                    }
                }
                else
                {
                    args.Channel.SendMessage("Utilisateur invalide");
                }
            }

            if (user != null)
            {
                args.Channel.SendMessage(user.Name + " à " + GetUserMoney(user.Id) + "$.");
            }
        }

        [Command("setmoney", "Permet de spécifier le montant possédé pas un usager. Disponnible pour faire des test")]
        [Parameter("Value", ParameterType.Required)]
        [Parameter("user", ParameterType.Optional)]
        public void SetMoney(CommandEventArgs args)
        {
            User user = null;
            string userArg = args.GetArg("user");
            long value;

            if (long.TryParse(args.GetArg("Value"), out value))
            {
                //trouver l'utilisateur
                if (args.GetArg("user") == "")
                {
                    user = args.User;
                }
                else
                {
                    ulong id = 0;
                    if (TryGetIdFromMention(userArg, ref id))
                    {
                        string idString = userArg.Substring(2, userArg.Length - 3);

                        user = args.Channel.GetUser(id);

                        if(user == null)
                        {
                            SetUserMoney(id, value);
                            args.Channel.SendMessage("Cette personne à maintenant " + GetUserMoney(user.Id) + "$!");
                        }
                    }
                    else
                    {
                        args.Channel.SendMessage("Utilisateur invalide");
                    }
                }

                if (user != null)
                {
                    SetUserMoney(user.Id, value);
                    args.Channel.SendMessage(user.Name + " à maintenant " + GetUserMoney(user.Id) + "$!");
                }
                
            }
            else
            {
                args.Channel.SendMessage("Veuillez entrez un entier valide.");
            }
        }

        public void SetGame(string gameName)
        {
            Program.WriteLine("Changement de jeu pour " + gameName, ConsoleColor.DarkMagenta);
            client.SetGame(gameName, GameType.Default, "data:image/jpeg;base64,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");
            Program.WriteLine("Le jeu à été changé pour " + client.CurrentGame.Name, ConsoleColor.DarkMagenta);
        }

        [Command("test", "Fonctionnalité mystérieuse utilisée par le développeur")]
        [Parameter("message", ParameterType.Required)]
        [Hidden()]
        public void Log(CommandEventArgs args)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            Console.Write(args.User.Name + " (");
            Console.Write(args.User.Id);
            Console.Write(") - " + args.Channel.Name + ": ");
            Console.ResetColor();
            Console.WriteLine(args.GetArg("message"));
        }

        [Command("say", "Fait parler le bot", "dit")]
        [Parameter("message", ParameterType.Required)]
        public void Say(CommandEventArgs args)
        {
            args.Channel.SendMessage(args.GetArg("message"));
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
            catch (Exception e)
            {
                noError = false;
            }
            return noError;
        }
    }
}
