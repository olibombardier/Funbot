using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Funbot
{
    class Person
    {
        public ulong Id { get; set; }
        public long Money { get; set; }

        public void Save(BinaryWriter stream)
        {
            stream.Write(Id);
            stream.Write(Money);
        }

        public static Person Read(BinaryReader stream)
        {
            Person p = new Person();
            p.Id = stream.ReadUInt64();
            p.Money = stream.ReadInt64();

            return p;
        }

        public override bool Equals(object obj)
        {
            return Id.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (int)Id;
        }
    }
}
