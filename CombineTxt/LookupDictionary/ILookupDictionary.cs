using System.Collections.Generic;

namespace CombineTxt.LookupDictionary
{
    public interface ILookupDictionary : IEnumerable<KeyValuePair<string, List<string>>>
    {
        void Add(string key, List<string> record);
        void Remove(string key);
        void Clear();
        bool ContainsKey(string key);
        List<string> this[string key] { get; }
    }
}