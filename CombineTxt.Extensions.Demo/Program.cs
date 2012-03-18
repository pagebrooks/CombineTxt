using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombineTxt.Extensions.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var factory = new SqlLookupDictionaryFactory())
            {
                CombineTxt.LookupDictionaryFactory = factory;
                CombineTxt.With("parent.txt")
                    .DefineKeyBy(s => s.Split('|')[1])
                    .RecordDelimitedBy(s => s.StartsWith("A"))
                    .JoinTo("child.txt")
                    .DefineKeyBy(s => s.Split('|')[1])
                    .WriteResultTo("output.txt");
            }
        }
    }
}
