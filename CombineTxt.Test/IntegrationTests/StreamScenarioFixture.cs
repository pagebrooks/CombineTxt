using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace CombineTxt.Test.IntegrationTests
{
    [TestFixture]
    public class StreamScenarioFixture
    {
        [Test]
        public void Join_Operations_With_Streams_Join_Files_Into_Output_Stream()
        {
            StringBuilder parentFile = new StringBuilder();
            parentFile.AppendLine(" A|1000|Acura|1990");
            parentFile.AppendLine(" B|1|Sub Record Information");
            parentFile.AppendLine(" A|1001|Nissan|1990");
            parentFile.AppendLine(" B|2|Sub Record Information");
            parentFile.AppendLine(" A|1002|Ford|1990");
            parentFile.AppendLine(" B|3|Sub Record Information");
            parentFile.AppendLine(" A|1003|Toyota|1990");
            parentFile.AppendLine(" B|4|Sub Record Information");
            MemoryStream parentStream = new MemoryStream(Encoding.UTF8.GetBytes(parentFile.ToString()));

            StringBuilder childFile = new StringBuilder();
            childFile.AppendLine("  C|1000|TS");
            childFile.AppendLine("  C|1000|MDX");
            childFile.AppendLine("  C|1001|Maxima");
            childFile.AppendLine("  C|1001|Pathfinder");
            childFile.AppendLine("  C|1002|Focus");
            childFile.AppendLine("  C|1005|Something");
            MemoryStream childStream = new MemoryStream(Encoding.UTF8.GetBytes(childFile.ToString()));

            MemoryStream outputStream = new MemoryStream();

            CombineTxt.With(parentStream)
                .DefineKeyBy(l => l.Split('|')[1])
                .ForEachLine(l => l.TrimStart())
                .RecordDelimitedByStartingWith("A")
                .JoinTo(childStream)
                .DefineKeyBy(l => l.Split('|')[1])
                .ForEachLine(l => l.TrimStart())
                .WriteResultTo(outputStream);

            parentFile.AppendLine(" A|1000|Acura|1990");
            parentFile.AppendLine(" B|1|Sub Record Information");
            parentFile.AppendLine(" A|1001|Nissan|1990");
            parentFile.AppendLine(" B|2|Sub Record Information");
            parentFile.AppendLine(" A|1002|Ford|1990");
            parentFile.AppendLine(" B|3|Sub Record Information");
            parentFile.AppendLine(" A|1003|Toyota|1990");
            parentFile.AppendLine(" B|4|Sub Record Information");

            childFile.AppendLine("  C|1000|TS");
            childFile.AppendLine("  C|1000|MDX");
            childFile.AppendLine("  C|1001|Maxima");
            childFile.AppendLine("  C|1001|Pathfinder");
            childFile.AppendLine("  C|1002|Focus");
            childFile.AppendLine("  C|1005|Something");
            List<string> lines = new List<string>
                                     {
                                         "A|1000|Acura|1990",
                                         "B|1|Sub Record Information",
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

            outputStream.Position = 0;
            StreamReader sr = new StreamReader(outputStream);
            string outputLine = null;
            int index = 0;
            while((outputLine = sr.ReadLine()) != null)
            {
                Assert.AreEqual(lines[index++], outputLine);
            }
        }
    }
}
