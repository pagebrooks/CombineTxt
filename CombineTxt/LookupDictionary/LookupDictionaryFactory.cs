namespace CombineTxt.LookupDictionary
{
    public class DefaultLookupDictionaryFactory : ILookupDictionaryFactory
    {
        public ILookupDictionary CreateLookupDictionary()
        {
            return new DefaultLookupDictionary();
        }
    }
}
