using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;


using Discord;
using DSLib.DiscordCommands;

namespace Funbot
{
    class BotDebug
    {
        private static object logLock = new object();

        private const ulong MyId = 202154315765383169;

        private const string roastStatsFileName = "roastStats.csv";
        private const string logFileName = "Funbot.log";

        private static int[] roastStats = new int[0];

        public static void InitDebug()
        {
            ReadRoastStats();
        }

        public static void StopDebug()
        {
            SaveRoastStats();
        }

        public static void ReadRoastStats()
        {
            List<int> linesRead = new List<int>();

            try
            {
                using(StreamReader reader = new StreamReader(roastStatsFileName)){
                    while (!reader.EndOfStream)
                    {
                        int value;
                        if(int.TryParse(reader.ReadLine().Trim(), out value))
                        {
                            linesRead.Add(value);
                        }
                        else
                        {
                            linesRead.Add(0);
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                BotDebug.Log("Le fichier \"" + roastStatsFileName + "\" n'a pas été trouvé", "FileError", ConsoleColor.Red);
            }

            roastStats = linesRead.ToArray();
        }

        public static void SaveRoastStats()
        {
            StreamWriter writer;
            try
            {
                using (writer = new StreamWriter(roastStatsFileName, false))
                {
                    foreach (int count in roastStats)
                    {
                        writer.WriteLine(count.ToString());
                    }
                }
            }
            catch (FileNotFoundException)
            {
                BotDebug.Log("Le fichier \"" + roastStatsFileName + "\" n'a pas été trouvé", "FileError", ConsoleColor.Red);
            }
        }

        public static void Log(string message, string logType = "log", ConsoleColor consoleColor = ConsoleColor.White)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("[");
            builder.Append(DateTime.Now.ToShortDateString() + " ");
            builder.Append(DateTime.Now.ToShortTimeString());
            builder.Append(" - ");
            builder.Append(logType);
            builder.Append("] ");

            builder.Append(message);

            Program.WriteLine(builder.ToString(), consoleColor);


            lock (logLock)
            {
                using (StreamWriter logWriter = new StreamWriter(logFileName, true))
                {
                    logWriter.WriteLine(builder.ToString());
                }
            }
        }

        public static void OnDiscordLog(object sender, LogMessageEventArgs args)
        {
            ConsoleColor messageColor = ConsoleColor.White;

            switch (args.Severity)
            {
                case LogSeverity.Error:
                    messageColor = ConsoleColor.Red;
                    break;

                case LogSeverity.Warning:
                    messageColor = ConsoleColor.Yellow;
                    break;

                case LogSeverity.Debug:
                    messageColor = ConsoleColor.DarkCyan;
                    break;
            }

            Log(args.Message, args.Severity.ToString(), messageColor);
        }

        [Command("getgames")]
        public async static Task GetGameList(CommandEventArgs args)
        {
            if (args.User.Id == MyId)
            {
                await args.Channel.SendFile(Program.gameFileName);
            }
        }

        [Command("updateGames")]
        public async static Task UpdateGames(CommandEventArgs args)
        {
            if (args.User.Id == MyId)
            {
                Message.Attachment[] attachments = args.Message.Attachments;
                if (attachments.Length == 1)
                {
                    WebClient webClient = new WebClient();

                    await Task.Run(
                        () => {
                            webClient.DownloadFile(attachments[0].Url, Program.gameFileName);
                        });

                    await args.Channel.SendMessage("Jeux mis à jour");
                }
            }
        }

        [Command("updateroasts")]
        public async static Task UpdateRoasts(CommandEventArgs args)
        {
            if (args.User.Id == MyId)
            {
                Message.Attachment[] attachments = args.Message.Attachments;
                if (attachments.Length == 1)
                {
                    WebClient webClient = new WebClient();

                    await Task.Run(
                        () =>
                        {
                            webClient.DownloadFile(attachments[0].Url, Program.roastFileName);
                        });
                    await args.Channel.SendMessage("Roasts mis à jour");
                }
            }
        }

        [Command("getroasts")]
        public async static Task GetRoastList(CommandEventArgs args)
        {
            if (args.User.Id == MyId)
            {
                await args.Channel.SendFile(Program.roastFileName);
            }
        }

        [Command("getstats")]
        public async static Task GetRoastStats(CommandEventArgs args)
        {
            SaveRoastStats();
            if (args.User.Id == MyId)
            {
                await args.Channel.SendFile(roastStatsFileName);
            }
        }

        [Command("getlog")]
        public async static Task GetLog(CommandEventArgs args)
        {
            if (args.User.Id == MyId)
            {
                await args.Channel.SendFile(logFileName);
            }
        }

        [Command("clearlog")]
        public async static Task ClearLog(CommandEventArgs args)
        {
            await GetLog(args);

            if(args.User.Id == MyId)
            {
                lock (logLock)
                {
                    File.Delete(logFileName);
                }
            }
        }

        public static void OnRoast(int roastIndex)
        {
            if (roastIndex >= roastStats.Length)
            {
                Array.Resize(ref roastStats, roastIndex + 1);
            }

            roastStats[roastIndex]++;
        }
    }
}
