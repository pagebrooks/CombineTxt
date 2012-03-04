using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace CombineTxt.Test
{
    [TestFixture]
    public class CombineTxtFixture
    {
        [Test]
        public void With_Creates_New_CombineTxtInfo_With_CombineTxt()
        {
            var c = CombineTxt.With("foo.txt");
            Assert.AreEqual(typeof(CombineTxtInfoFixture), c.GetType());
        }
    }
}
