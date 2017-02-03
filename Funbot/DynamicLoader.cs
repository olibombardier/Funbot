using System;
using System.Reflection;

namespace Funbot
{
    static class DynamicLoader
    {
        public static void LoadCommandsFromDll(Bot bot, string filename)
        {
            Program.Write("Chargement des commandes de \"");
            Program.Write(filename);
            Program.WriteLine("\".");

            Assembly DLL = null;
            try
            {
                DLL = Assembly.LoadFile(filename);
            }
            catch (Exception e)
            {
                Program.WriteError("Impossible de charger \"" + filename + "\". (" + e.Message + ")");
            }
            
            if(DLL != null)
            {
                foreach (Type t in DLL.GetExportedTypes())
                {
                    bot.CreateCommandsFromClass(t);
                }
            }
        }
    }
}
