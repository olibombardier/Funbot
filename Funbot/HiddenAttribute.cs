using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funbot
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HiddenAttribute : Attribute
    {
    }
}
