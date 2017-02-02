using System;
using System.Reflection;

namespace Funbot
{
    static class DynamicLoader
    {
        public static void LoadCommandsFromDll(Bot bot, string filename)
        {
            Assembly DLL = Assembly.LoadFile(filename);

            foreach(Type t in DLL.GetExportedTypes())
            {
                foreach(MethodInfo method in t.GetMethods())
            }
        }
    }
}
