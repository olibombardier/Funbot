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
        public DiscordClient DiscorClient
        {
            get { return client; }
        }

        private List<CommandHelp> commandHelp = new List<CommandHelp>();

        private Dictionary<ulong, Person> users = new Dictionary<ulong, Person>();

        private static readonly string[] answerHello = { "Salut!", "Hello!", "Coucou!", "Hey!", "Hello!", "Yo!", "Hello! :smile:" };
        private static readonly string[] answerBye = { "Bye!", "Au revoir!", "À la procahine!", "Bye bye!" };

        public static readonly Random rand = new Random((int)DateTime.Now.Ticks);

        public static Bot botInstance { get; private set; }

        static Bot()
        {
            botInstance = new Bot();

            botInstance.client = new DiscordClient(x => { x.LogLevel = LogSeverity.Info; });
            botInstance.client.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
            });

            botInstance.CreateCommandsFromClass(typeof(Bot), botInstance);
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

        public void CreateCommandsFromClass(Type classType, object classInstence = null)
        {
            object instence = classInstence;
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
                        if(instence == null)
                        {
                            try
                            {
                                instence = Activator.CreateInstance(classType);
                            }
                            catch(Exception e)
                            {
                                Program.WriteError("Impossible de créer une instance de \"" + classType.Name + "\". (" + e.Message + ")");
                            }
                        }

                        builder.Do((Action<CommandEventArgs>)Delegate.CreateDelegate(typeof(Action<CommandEventArgs>), instence, method));
                        Console.WriteLine("Commend added: {0}.", command.Name);

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
            client.SetGame(gameName);
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
            catch (Exception)
            {
                noError = false;
            }
            return noError;
        }
    }
}
