using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funbot
{
    public class CommandHelp
    {
        public string CommandName { get; set; }
        public string[] Aliases { get; set; }
        public string HelpText { get; set; }
    }
}
