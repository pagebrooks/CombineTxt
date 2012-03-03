using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombineTxt
{
    public class CombineTxtInfo
    {
        private readonly CombineTxt _manipulation;

        public CombineTxtInfo(CombineTxt manipulation)
        {
            _manipulation = manipulation;
        }

        public CombineTxt DefineKeyBy(Func<string, string> action)
        {
            KeyDefinition = action;
            _manipulation.KeyInfo = this;
            return _manipulation;
        }

        public CombineTxt DoNotDefineKey()
        {
            KeyDefinition = l => l;
            _manipulation.KeyInfo = this;
            return _manipulation;
        }

        public Func<string, string> KeyDefinition { get; private set; }
    }
}
