using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fairdao.extensions.shared.IndexedDB
{
    /// <summary>
    /// Defines an Index for a given object store.
    /// </summary>
    public class IDBIndex
    {
        private struct DbFunctions
        {
            public const string CountFromIndex = "countFromIndex";
            public const string CountFromIndexByKeyRange = "countFromIndexByKeyRange";
            public const string GetFromIndex = "getFromIndex";
            public const string GetAllFromIndex = "getAllFromIndex";
            public const string GetAllFromIndexByKeyRange = "getAllFromIndexByKeyRange";
            public const string GetAllFromIndexByArrayKey = "getAllFromIndexByArrayKey";
            public const string GetKeyFromIndex = "getKeyFromIndex";
            public const string GetAllKeysFromIndex = "getAllKeysFromIndex";
            public const string GetAllKeysFromIndexByKeyRange = "getAllKeysFromIndexByKeyRange";
            public const string GetAllKeysFromIndexByArrayKey = "getAllKeysFromIndexByArrayKey";
            public const string QueryFromIndex = "queryFromIndex";
        }

        private IDBDatabase _idbDatabase;
        private IDBObjectStore _idbStore;

        /// <summary>
        /// The name of the index.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// the identifier for the property in the object/record that is saved and is to be indexed.
        /// can be multiple properties separated by comma
        /// if null will default to index name
        /// </summary>
        public string KeyPath { get; }

        /// <summary>
        /// Affects how the index behaves when the result of evaluating the index's key path yields an array.
        /// If true, there is one record in the index for each item in an array of keys.
        /// If false, then there is one record for each key that is an array.
        /// </summary>
        /// <value></value>
        public bool MultiEntry { get; }

        /// <summary>
        /// Only use for indexes
        /// If true, this index does not allow duplicate values for a key.
        /// </summary>
        public bool Unique { get; }

        /// <summary>
        /// Only use for indexes
        /// If true, this index does not allow duplicate values for a key.
        /// </summary>
        public IDBObjectStore ObjectStore => _idbStore;

        /// <summary>
        ///
        /// </summary>
        /// <param name="idbManager"></param>
        public IDBIndex(IDBObjectStore idbStore, string name, string keyPath, bool multiEntry = false, bool unique = false)
        {
            if (idbStore.Indexes.Any(i => i.Name == name))
            {
                throw new IDBException($"Store {idbStore.Name}, Index {name} already exists");
            }

            Name = name;
            KeyPath = keyPath;
            MultiEntry = multiEntry;
            Unique = unique;

            idbStore.Indexes.Add(this);

            _idbStore = idbStore;
            _idbDatabase = idbStore.IDBDatabase;
        }

        /// <summary>
        /// Count records in Index
        /// </summary>
        /// <returns></returns>
        public async Task<int> Count()
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<int>(DbFunctions.CountFromIndex, _idbStore.Name, Name);
        }

        /// <summary>
        /// Count records in Index
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public async Task<int> Count<TKey>(TKey key)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<int>(DbFunctions.CountFromIndex, _idbStore.Name, Name, key);
        }

        /// <summary>
        /// Count records in Index
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public async Task<int> Count<TKey>(IDBKeyRange<TKey> key)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<int>(DbFunctions.CountFromIndexByKeyRange, _idbStore.Name, Name, key.lower, key.upper, key.lowerOpen, key.upperOpen);
        }

        /// <summary>
        /// Returns the first record that matches a query against a given index
        /// </summary>
        /// <param name="queryValue"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<TResult> Get<TKey, TResult>(TKey queryValue)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<TResult>(DbFunctions.GetFromIndex, _idbStore.Name, Name, queryValue);
        }

        /// <summary>
        /// Gets all of the records that match a given query in the specified index.
        /// </summary>
        /// <param name="count"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> GetAll<TResult>(int? count = null)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.GetAllFromIndex, _idbStore.Name, Name, null, count);
        }

        /// <summary>
        /// Gets all of the records that match a given query in the specified index.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> GetAll<TKey, TResult>(TKey key, int? count = null)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.GetAllFromIndex, _idbStore.Name, Name, key, count);
        }

        /// <summary>
        /// Gets all of the records that match a given query in the specified index.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> GetAll<TKey, TResult>(IDBKeyRange<TKey> key, int? count = null)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.GetAllFromIndexByKeyRange, _idbStore.Name, Name, key.lower, key.upper, key.lowerOpen, key.upperOpen, count);
        }

        /// <summary>
        /// Gets all of the records that match a given query in the specified index.
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> GetAll<TKey, TResult>(TKey[] key)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.GetAllFromIndexByArrayKey, _idbStore.Name, Name, key);
        }

        /// <summary>
        /// Returns the first record keys that matches a query against a given index
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="searchQuery">an instance of StoreIndexQuery</param>
        /// <returns></returns>
        public async Task<TResult> GetKey<TKey, TResult>(TKey key)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<TResult>(DbFunctions.GetKeyFromIndex, _idbStore.Name, Name, key);
        }

        /// <summary>
        /// Gets all of the records keys that match a given query in the specified index.
        /// </summary>
        /// <param name="count"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> GetAllKeys<TResult>(int? count = null)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.GetAllKeysFromIndex, _idbStore.Name, Name, null, count);
        }

        /// <summary>
        /// Gets all of the records keys that match a given query in the specified index.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> GetAllKeys<TKey, TResult>(TKey key, int? count = null)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.GetAllKeysFromIndex, _idbStore.Name, Name, key, count);
        }

        /// <summary>
        /// Gets all of the records that match a given query in the specified index.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> GetAllKeys<TKey, TResult>(IDBKeyRange<TKey> key, int? count = null)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.GetAllKeysFromIndexByKeyRange, _idbStore.Name, Name, key.lower, key.upper, key.lowerOpen, key.upperOpen, count);
        }

        /// <summary>
        /// Gets all of the records that match a given query in the specified index.
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> GetAllKeys<TKey, TResult>(TKey[] key)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.GetAllKeysFromIndexByArrayKey, _idbStore.Name, Name, key);
        }

        /// <summary>
        /// Gets all of the records using a filter expression
        /// </summary>
        /// <param name="filter">expresion that evaluates to true/false, each record es passed to "obj" parameter</param>
        /// <param name="count"></param>
        /// <param name="skip"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> Query<TResult>(string filter, int? count = null, int? skip = null)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.QueryFromIndex, _idbStore.Name, Name, null, filter, count, skip);
        }

        /// <summary>
        /// Gets all of the records using a filter expression
        /// </summary>
        /// <param name="filter">expresion that evaluates to true/false, each record es passed to "obj" parameter</param>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <param name="skip"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> Query<TKey, TResult>(string filter, TKey key, int? count = null, int? skip = null)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.QueryFromIndex, _idbStore.Name, Name, key, filter, count, skip);
        }

        /// <summary>
        /// Gets all of the records using a filter expression
        /// </summary>
        /// <param name="filter">expresion that evaluates to true/false, each record es passed to "obj" parameter</param>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <param name="skip"></param>
        /// <param name="order">顺序next,倒序prev</param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> Query<TKey, TResult>(string filter, IDBKeyRange<TKey> key, int? count = null, int? skip = null, string order = "next")
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.QueryFromIndex, _idbStore.Name, Name, key, filter, count, skip, order);
        }
    }
}
