using System;
using System.Reflection;
using System.IO;
using System.Timers;

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

        private static readonly string[] answerHello = { "Salut!", "Hello!", "Coucou!", "Hey!", "Hello!", "Yo!", "Hello! :smile:" };
        private static readonly string[] answerBye = { "Bye!", "Au revoir!", "À la procahine!", "Bye bye!" };

        public static readonly Random rand = new Random((int)DateTime.Now.Ticks);

        public static Bot botInstance { get; private set; }

        static Bot()
        {
            botInstance = new Bot();
            
            botInstance.CreateCommandsFromClass(typeof(Bot), botInstance);
        }

        private Bot()
        {
            client = new DiscordClient(x => { x.LogLevel = LogSeverity.Info; });
            client.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
            });
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
