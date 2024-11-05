namespace fairdao.extensions.shared.IndexedDB
{
    public class IDBException : System.Exception
    {
        public IDBException(string message) : base(message) { }
    }

    public class IDBNotFoundError : IDBException
    {
        public IDBNotFoundError(string message) : base(message) { }
    }

}