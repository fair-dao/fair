namespace fairdao.extensions.shared.IndexedDB
{
    public class IDBKeyRange<TKey>
    {
        public TKey? lower { get; set; }
        public TKey? upper { get; set; }
        public bool lowerOpen { get; set; }
        public bool upperOpen { get; set; }
    }
}