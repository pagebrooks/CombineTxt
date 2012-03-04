using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace CombineTxt.Test
{
    [TestFixture]
    public class CombineTxtInfoFixture
    {
        [Test]
        public void DefineKeyBy_Sets_KeyDefinition()
        {
            var info = CombineTxt.With("foo.txt");
            Func<string, string> func = s => string.Empty;
            info.DefineKeyBy(func);
            Assert.AreSame(func, info.KeyDefinition);
        }

        [Test]
        public void DoNotDefineKey_Sets_Identity_Function_For_KeyDefinition()
        {
            var info = CombineTxt.With("foo.txt");
            info.DoNotDefineKey();
            Assert.AreEqual("foo", info.KeyDefinition("foo"));
        }
    }
}
