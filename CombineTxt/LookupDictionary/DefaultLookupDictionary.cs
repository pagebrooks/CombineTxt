﻿using System.Collections;
using System.Collections.Generic;

namespace CombineTxt.LookupDictionary
{
    public class DefaultLookupDictionary : ILookupDictionary
    {
        private readonly Dictionary<string, List<string>> _dictionary;

        public DefaultLookupDictionary()
        {
            _dictionary = new Dictionary<string, List<string>>();
        }

        public void Add(string key, string record)
        {
            if (_dictionary.ContainsKey(key))
            {
                _dictionary[key].Add(record);
            }
            else
            {
                _dictionary.Add(key, new List<string> { record });
            }
        }

        public void Remove(string key)
        {
            _dictionary.Remove(key);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public List<string> this[string key]
        {
            get
            {
                return _dictionary[key];
            }
        }

        public IEnumerator<KeyValuePair<string, List<string>>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
