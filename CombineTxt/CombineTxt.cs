using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CombineTxt
{
    public class CombineTxt
    {
        private readonly string _fileName;
        private readonly Encoding _encoding;
        private Func<string, string> _forEachFunc;
        private Func<string, bool> _newRecordFunc;
        private Action<List<string>> _unMatchedFunc;

        private CombineTxt(string fileName, Encoding encoding)
        {
            _newRecordFunc = s => true;
            _forEachFunc = s => s;
            _unMatchedFunc = null;
            _fileName = fileName;
            _encoding = encoding;
        }

        public static CombineTxtInfo With(string fileName)
        {
            return With(fileName, Encoding.UTF8);
        }

        public static CombineTxtInfo With(string fileName, Encoding encoding)
        {
            CombineTxt manipulation = new CombineTxt(fileName, encoding);
            return new CombineTxtInfo(manipulation);
        }

        public CombineTxt ForEachLine(Func<string, string> action)
        {
            _forEachFunc = action;
            return this;
        }

        public CombineTxt RecordDelimitedByStartingWith(string startsWith)
        {
            _newRecordFunc = s => s.StartsWith(startsWith);
            return this;
        }

        public CombineTxt RecordDelimitedBy(Func<string, bool> newRecord)
        {
            _newRecordFunc = newRecord;
            return this;
        }

        public CombineTxtInfo JoinTo(string fileName, Action<List<string>> noMatch = null)
        {
            return JoinTo(fileName, Encoding.UTF8, noMatch);
        }

        public CombineTxtInfo JoinTo(string fileName, Encoding encoding, Action<List<string>> noMatch = null)
        {
            CombineTxt manipulation = new CombineTxt(fileName, encoding);
            manipulation.Parent = this;
            manipulation._unMatchedFunc = noMatch;
            return new CombineTxtInfo(manipulation);
        }

        public void WriteResultTo(string fileName)
        {
            WriteResultTo(fileName, Encoding.UTF8);
        }

        public void WriteResultTo(string fileName, Encoding encoding)
        {
            var children = new List<Dictionary<string, List<string>>>();
            CombineTxt m = this;

            while (m.Parent != null)
            {
                var matchDictionary = new Dictionary<string, List<string>>();

                using (StreamReader sr = new StreamReader(m._fileName, m._encoding))
                {
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var filtered = m._forEachFunc(line);
                        var key = m.KeyInfo.KeyDefinition(filtered);
                        if (matchDictionary.ContainsKey(key))
                        {
                            matchDictionary[key].Add(filtered);
                        }
                        else
                        {
                            matchDictionary.Add(key, new List<string> { filtered });
                        }
                    }

                    sr.Close();
                }

                children.Add(matchDictionary);
                m = m.Parent;
            }

            using (StreamWriter sw = new StreamWriter(fileName, false, encoding))
            using (StreamReader sr = new StreamReader(m._fileName, m._encoding))
            {
                string line = null;
                string key = null;
                string previousKey = null;
                while ((line = sr.ReadLine()) != null)
                {
                    var filtered = m._forEachFunc(line);
                    if (m._newRecordFunc(filtered))
                    {
                        key = m.KeyInfo.KeyDefinition(filtered);
                    }

                    if (previousKey != null && previousKey != key)
                    {
                        WriteChildren(sw, previousKey, children);
                    }

                    sw.WriteLine(filtered);
                    previousKey = key;
                }

                WriteChildren(sw, previousKey, children);
                sw.Close();
            }

            m = this;
            int i = 0;
            while (m.Parent != null)
            {
                List<string> unMatchedItems = new List<string>();
                foreach (var matchItem in children[i++])
                {
                    unMatchedItems.AddRange(matchItem.Value);
                }
                if (m._unMatchedFunc != null)
                {
                    m._unMatchedFunc(unMatchedItems);
                }

                m = m.Parent;
            }

            return;
        }

        private void WriteChildren(StreamWriter sw, string key, IEnumerable<Dictionary<string, List<string>>> children)
        {
            foreach (var matchDictionary in children)
            {
                if (matchDictionary.ContainsKey(key))
                {
                    foreach (var childLine in matchDictionary[key])
                    {
                        sw.WriteLine(childLine);
                    }

                    matchDictionary.Remove(key);
                    sw.Flush();
                }
            }
        }

        public CombineTxt Parent { get; set; }
        public CombineTxtInfo KeyInfo { get; set; }

    }
}
