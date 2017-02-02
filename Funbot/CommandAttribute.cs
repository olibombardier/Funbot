using System;

namespace Funbot
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public string Name { get; set; }
        public string[] Aliases { get; set; }
        public string HelpText { get; set; }

        public CommandAttribute(string name, string helpText, params string[] aliases)
        {
            Name = name;
            Aliases = aliases;
            HelpText = helpText;
        }
    }
}
