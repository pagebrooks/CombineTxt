using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace CombineTxt.Test.IntegrationTests
{
    [TestFixture]
    public class SimpleJoinScenarioFixture
    {
        [Test]
        public void Validate_Simple_Join_Scenario()
        {
            string path = @"..\..\..\IntegrationTests\SimpleJoinScenarioFixture";
            CombineTxt.With(Path.Combine(path, "ParentFile.txt"))
                .DefineKeyBy(l => l.Split('|')[1])
                .RecordDelimitedByStartingWith("A")
                .JoinTo(Path.Combine(path, "ChildFile1.txt"))
                .DefineKeyBy(l => l.Split('|')[1])
                .WriteResultTo(Path.Combine(path, "Output1.txt"));

            List<string> lines = new List<string>
                                     {
                                         "A|1000|Acura|1990",
                                         "B|1|Sub Record Information",
                                         "C|1000|TSX",
                                         "C|1000|TS",
                                         "C|1000|MDX",
                                         "A|1001|Nissan|1990",
                                         "B|2|Sub Record Information",
                                         "C|1001|Maxima",
                                         "C|1001|Pathfinder",
                                         "A|1002|Ford|1990",
                                         "B|3|Sub Record Information",
                                         "C|1002|Focus",
                                         "A|1003|Toyota|1990",
                                         "B|4|Sub Record Information"
                                     };

            var outputLines = File.ReadAllLines(Path.Combine(path, "Output1.txt"));
            int index = 0;
            foreach (var line in outputLines)
            {
                Assert.AreEqual(lines[index++], line);
            }
        }

        [Test]
        public void Format_File_Lines()
        {
            string path = @"..\..\..\IntegrationTests\SimpleJoinScenarioFixture";
            CombineTxt.With(Path.Combine(path, "ParentFile.txt"))
                .DoNotDefineKey()
                .ForEachLine(l => l.Replace("|", "$"))
                .RecordDelimitedByStartingWith("A")
                .WriteResultTo(Path.Combine(path, "Output2.txt"));

            List<string> lines = new List<string>
                                     {
                                         "A$1000$Acura$1990",
                                         "B$1$Sub Record Information",
                                         "A$1001$Nissan$1990",
                                         "B$2$Sub Record Information",
                                         "A$1002$Ford$1990",
                                         "B$3$Sub Record Information",
                                         "A$1003$Toyota$1990",
                                         "B$4$Sub Record Information"
                                     };

            var outputLines = File.ReadAllLines(Path.Combine(path, "Output2.txt"));
            int index = 0;
            foreach (var line in outputLines)
            {
                Assert.AreEqual(lines[index++], line);
            }
        }
    }
}
