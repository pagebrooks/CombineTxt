using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CombineTxt.LookupDictionary;

namespace CombineTxt
{
    public class CombineTxt
    {
        private static ILookupDictionaryFactory _lookupDictionaryFactory;

        private readonly StreamReaderProvider _readerProvider;
        private Func<string, string> _forEachFunc = s => s;
        private Func<string, bool> _newRecordFunc = s => true;
        private Action<List<string>> _unMatchedFunc = null;

        private CombineTxt(string fileName, Encoding encoding)
        {
            _readerProvider = new StreamReaderProvider(fileName, encoding);
        }

        public CombineTxt(Stream stream, Encoding encoding)
        {
            _readerProvider = new StreamReaderProvider(stream, encoding);
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

        public static CombineTxtInfo With(Stream stream)
        {
            return With(stream, Encoding.UTF8);
        }

        public static CombineTxtInfo With(Stream stream, Encoding encoding)
        {
            CombineTxt manipulation = new CombineTxt(stream, encoding);
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

        public CombineTxtInfo JoinTo(Stream stream, Action<List<string>> noMatch = null)
        {
            return JoinTo(stream, Encoding.UTF8, noMatch);
        }

        public CombineTxtInfo JoinTo(Stream stream, Encoding encoding, Action<List<string>> noMatch = null)
        {
            CombineTxt manipulation = new CombineTxt(stream, encoding);
            manipulation.Parent = this;
            manipulation._unMatchedFunc = noMatch;
            return new CombineTxtInfo(manipulation);
        }

        public void WriteResultTo(string fileName, Encoding encoding)
        {
            StreamWriterProvider writerProvider = new StreamWriterProvider(fileName, encoding);
            InternalWriteResultTo(writerProvider);
        }

        public void WriteResultTo(string fileName)
        {
            WriteResultTo(fileName, Encoding.UTF8);
        }

        public void WriteResultTo(Stream stream)
        {
            WriteResultTo(stream, Encoding.UTF8);
        }

        public void WriteResultTo(Stream stream, Encoding encoding)
        {
            StreamWriterProvider writerProvider = new StreamWriterProvider(stream, encoding, true);
            InternalWriteResultTo(writerProvider);
        }

        private void InternalWriteResultTo(StreamWriterProvider writerProvider)
        {
            var children = new List<ILookupDictionary>();
            CombineTxt m = this;

            while (m.Parent != null)
            {
                var matchDictionary = LookupDictionaryFactory.CreateLookupDictionary();

                using (StreamReader sr = _readerProvider.GetStreamReader())
                {
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var filtered = m._forEachFunc(line);
                        var key = m.KeyInfo.KeyDefinition(filtered);
                        matchDictionary.Add(key, filtered);
                    }

                    sr.Close();
                }

                children.Add(matchDictionary);
                m = m.Parent;
            }

            StreamWriter sw = writerProvider.GetStreamWriter();
            using (StreamReader sr = m._readerProvider.GetStreamReader())
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

                // StreamWriters assume they own the stream so they
                // close the stream when they are disposed of.
                sw.Flush();
                if (!writerProvider.KeepStreamOpen)
                {
                    sw.Close();
                    sw.Dispose();
                }
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
        }

        private void WriteChildren(StreamWriter sw, string key, IEnumerable<ILookupDictionary> children)
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
                }
            }
        }

        public CombineTxt Parent { get; set; }
        public CombineTxtInfo KeyInfo { get; set; }

        public static ILookupDictionaryFactory LookupDictionaryFactory
        {
            get { return _lookupDictionaryFactory ?? (_lookupDictionaryFactory = new DefaultLookupDictionaryFactory()); }
            set { _lookupDictionaryFactory = value; }
        }
    }
}
