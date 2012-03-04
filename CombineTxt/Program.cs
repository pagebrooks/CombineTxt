using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CombineTxt
{
    class Program
    {
        static void Main(string[] args)
        {


            CombineTxt.With(@"..\..\..\Docs\ParentFile.txt")
                .DefineKeyBy(l => l.Split('|')[1])
                .ForEachLine(l => l.Trim())
                .RecordDelimitedByStartingWith("A")
                .JoinTo(@"..\..\..\Docs\ChildFile1.txt")
                .DefineKeyBy(l => l.Split('|')[1])
                .ForEachLine(l => l.Trim())
                .JoinTo(@"..\..\..\Docs\ChildFile2.txt")
                .DefineKeyBy(l => l.Split('-')[1])
                .WriteResultTo(@"..\..\..\Docs\Output1.txt");

            CombineTxt.With(@"..\..\..\Docs\ParentFile.txt")
                        .DoNotDefineKey()
                        .ForEachLine(l => {
                                             var trimmedLine = l.Trim();
                                             return trimmedLine.Substring(2, trimmedLine.Length - 2);
                                          })
                        .WriteResultTo(@"..\..\..\Docs\Output2.txt");

            Console.Read();
        }
    }
}
