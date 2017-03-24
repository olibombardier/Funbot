using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funbot
{
    public class EnvironmentRandom
    {
        public EnvironmentRandom()
        {
            createdTime = DateTime.Now;
            lastNextTime = createdTime;
            counter = 0;
        }

        private DateTime createdTime;
        private DateTime lastNextTime;
        private uint counter;

        public int Next()
        {
            return Math.Abs((int)NextUInt32());
        }

        public uint NextUInt32()
        {
            DateTime now = DateTime.Now;

            uint val = (uint)createdTime.GetHashCode() ^
                (uint)lastNextTime.GetHashCode() ^
                (uint)now.GetHashCode() ^
                counter ^
                (uint)Environment.TickCount
                ^ (uint)Environment.WorkingSet;

            lastNextTime = now;
            counter++;
            return val;
        }

        public long NextInt64()
        {
            return Math.Abs((long)NextUInt64());
        }

        public ulong NextUInt64()
        {
            DateTime now = DateTime.Now;

            ulong val = ((ulong)createdTime.GetHashCode() | ((ulong)lastNextTime.GetHashCode() << 32)) ^
                ((ulong)now.GetHashCode() | ((ulong)counter << 32)) ^
                ((ulong)Environment.TickCount | ((ulong)Environment.WorkingSet << 32));

            lastNextTime = now;
            counter++;
            return val;
        }

        public int Next(int max)
        {
            return Next() % max;
        }

        public int Next(int min, int max)
        {
            return Next(max) + min;
        }

    }
}
