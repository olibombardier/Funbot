using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Discord;
using DSLib.DiscordCommands;

namespace Funbot
{
    class BotDebug
    {
        private static StreamWriter logWriter;

        private const ulong MyId = 202154315765383169;

        private const string roastStatsFileName = "roastStats.csv";
        private const string logFileName = "Funbot.log";

        private static int[] roastStats = new int[0];

        public static void InitDebug()
        {
            logWriter = new StreamWriter(logFileName, true);

            ReadRoastStats();
        }

        public static void StopDebug()
        {
            if (logWriter != null)
            {
                logWriter.Close();
            }

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

            if (logWriter != null)
            {
                lock (logWriter)
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

        public static void OnRoast(int roastIndex)
        {
            if (roastIndex >= roastStats.Length)
            {
                int[] newList = new int[roastIndex];
                roastStats.CopyTo(newList, 0);
                roastStats = newList;
            }

            roastStats[roastIndex]++;
        }
    }
}
