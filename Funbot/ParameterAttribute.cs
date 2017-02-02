using System;
using Discord.Commands;

namespace Funbot
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    class ParameterAttribute : Attribute
    {
        public string Name { get; set; }
        public ParameterType Type { get; set; }

        public ParameterAttribute(string name, ParameterType type)
        {
            Name = name;
            Type = type;
        }
    }
}
