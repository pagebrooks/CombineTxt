using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace CombineTxt.Test.IntegrationTests
{
    [TestFixture]
    public class NoMatchScenarioFixture
    {
        [Test]
        public void NoMatchItems_Function_Is_Called_With_List_Of_Non_Matching_Items()
        {
            List<string> noMatchItems = null;
            string path = @"..\..\..\IntegrationTests\NoMatchScenarioFixture";
            CombineTxt.With(Path.Combine(path, "ParentFile.txt"))
                .DefineKeyBy(l => l.Split('|')[1])
                .JoinTo(Path.Combine(path, "ChildFile1.txt"), i => noMatchItems = i)
                .DefineKeyBy(l => l.Split('|')[1])
                .WriteResultTo(Path.Combine(path, "Output1.txt"));

            Assert.AreEqual(2, noMatchItems.Count);
            Assert.AreEqual("C|1005|Something", noMatchItems[0]);
            Assert.AreEqual("C|1006|Else", noMatchItems[1]);

        }
    }
}
