using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace fairdao.extensions.shared.IndexedDB
{
    /// <summary>
    /// Defines a store to add to database
    /// </summary>
    public class IDBObjectStore
    {
        private struct DbFunctions
        {
            public const string Count = "count";
            public const string CountByKeyRange = "countByKeyRange";
            public const string Get = "get";
            public const string GetAll = "getAll";
            public const string GetAllByKeyRange = "getAllByKeyRange";
            public const string GetAllByArrayKey = "getAllByArrayKey";
            public const string GetKey = "getKey";
            public const string GetAllKeys = "getAllKeys";
            public const string GetAllKeysByKeyRange = "getAllKeysByKeyRange";
            public const string GetAllKeysByArrayKey = "getAllKeysByArrayKey";
            public const string Query = "query";
            public const string CountFromIndex = "countFromIndex";
            public const string CountFromIndexByKeyRange = "countFromIndexByKeyRange";
            public const string Add = "add";
            public const string Put = "put";
            public const string Delete = "delete";
            public const string BatchAdd = "batchAdd";
            public const string BatchPut = "batchPut";
            public const string BatchDelete = "batchDelete";
            public const string ClearStore = "clearStore";
        }

        private IDBDatabase _idbDatabase;
        private List<IDBIndex> _indexes = new List<IDBIndex>();

        /// <summary>
        /// The name for the store
        /// </summary>
        public string Name { get; init; } = "";

        /// <summary>
        /// the identifier for the property in the object/record that is saved and is to be indexed.
        /// can be multiple properties separated by comma
        /// If this property is null, the application must provide a key for each modification operation.
        /// </summary>
        public string KeyPath { get; init; }

        /// <summary>
        /// If true, the object store has a key generator. Defaults to false.
        /// Note that every object store has its own separate auto increment counter.
        /// </summary>
        /// <value></value>
        public bool AutoIncrement { get; init; }

        /// <summary>
        /// Provides a set of additional indexes if required.
        /// </summary>
        public List<IDBIndex> Indexes => _indexes;

        /// <summary>
        /// IDMManager
        /// </summary>
        public IDBDatabase IDBDatabase => _idbDatabase;

        /// <summary>
        /// Add new ObjectStore definition
        /// </summary>
        /// <param name="idbDatabase"></param>
        public IDBObjectStore(IDBDatabase idbDatabase)
        {
            _idbDatabase = idbDatabase;
            _idbDatabase.ObjectStores.Add(this);
        }

        /// <summary>
        /// Get index by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDBIndex Index(string indexName)
        {
            var index = _indexes.Find(i => i.Name == indexName);
            if (index == null)
            {
                throw new IDBNotFoundError($"Store {Name}, index {indexName} does not exists");
            }
            return index;
        }

        /// <summary>
        /// Count records in ObjectStore
        /// </summary>
        /// <returns></returns>
        public async Task<int> Count()
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<int>(DbFunctions.Count, Name);
        }

        /// <summary>
        /// Count records in ObjectStore
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public async Task<int> Count<TKey>(TKey key)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<int>(DbFunctions.Count, Name, key);
        }

        /// <summary>
        /// Count records in ObjectStore
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public async Task<int> Count<TKey>(IDBKeyRange<TKey> key)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<int>(DbFunctions.CountByKeyRange, Name, key.lower, key.upper, key.lowerOpen, key.upperOpen);
        }

        /// <summary>
        /// Retrieve a record by Key
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="key">the key of the record</param>
        /// <returns></returns>
        public async Task<TResult?> Get<TKey, TResult>(TKey key)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<TResult?>(DbFunctions.Get, Name, key);
        }

        /// <summary>
        /// Gets all of the records in a given store.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> GetAll<TResult>(int? count = null)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.GetAll, Name, null, count);
        }

        /// <summary>
        /// Gets all of the records by Key in a given store.
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> GetAll<TKey, TResult>(TKey key, int? count = null)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.GetAll, Name, key, count);
        }

        /// <summary>
        /// Gets all of the records by KeyRange in a given store.
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> GetAll<TKey, TResult>(IDBKeyRange<TKey> key, int? count = null)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.GetAllByKeyRange, Name, key.lower, key.upper, key.lowerOpen, key.upperOpen, count);
        }

        /// <summary>
        /// Gets all of the records by ArrayKey in a given store.
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> GetAll<TKey, TResult>(TKey[] key)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.GetAllByArrayKey, Name, key);
        }

        /// <summary>
        /// Retrieve a record key by Key
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="key">the key of the record</param>
        /// <returns></returns>
        public async Task<TResult?> GetKey<TKey, TResult>(TKey key)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<TResult?>(DbFunctions.GetKey, Name, key);
        }

        /// <summary>
        /// Gets all of the records keys in a given store.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> GetAllKeys<TResult>(int? count = null)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.GetAllKeys, Name, null, count);
        }

        /// <summary>
        /// Gets all of the records kesy by Key in a given store.
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> GetAllKeys<TKey, TResult>(TKey key, int? count = null)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.GetAllKeys, Name, key, count);
        }

        /// <summary>
        /// Gets all of the records by KeyRange in a given store.
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> GetAllKeys<TKey, TResult>(IDBKeyRange<TKey> key, int? count = null)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.GetAllKeysByKeyRange, Name, key.lower, key.upper, key.lowerOpen, key.upperOpen, count);
        }

        /// <summary>
        /// Gets all of the records by ArrayKey in a given store.
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public async Task<List<TResult>> GetAllKeys<TKey, TResult>(TKey[] key)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.GetAllKeysByArrayKey, Name, key);
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
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.Query, Name, null, filter, count, skip);
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
        public async Task<List<TResult>> Query<TKey, TResult>(string Name, string filter, TKey key, int? count = null, int? skip = null)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.Query, Name, key, filter, count, skip);
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
        public async Task<List<TResult>> Query<TKey, TResult>(string filter, IDBKeyRange<TKey> key, int? count = null, int? skip = null)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<List<TResult>>(DbFunctions.Query, Name, key, filter, count, skip);
        }

        /// <summary>
        /// Adds a new record/object to the specified ObjectStore
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public async Task Add<TData>(TData data) where TData : notnull
        {
            await _idbDatabase.EnsureIsOpen();
            await _idbDatabase.CallJavascript(DbFunctions.Add, Name, data);
        }

        /// <summary>
        /// Adds a new record/object to the specified ObjectStore
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public async Task<TKey> Add<TData, TKey>(TData data)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<TKey>(DbFunctions.Add, Name, data);
        }

        /// <summary>
        /// Adds a new record/object to the specified ObjectStore
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public async Task<TKey> Add<TData, TKey>(TData data, TKey key)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<TKey>(DbFunctions.Add, Name, data, key);
        }

        /// <summary>
        /// Updates and existing record
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public async Task Put<TData>(TData data) where TData : notnull
        {
            await _idbDatabase.EnsureIsOpen();
            await _idbDatabase.CallJavascript(DbFunctions.Put, Name, data);
        }

        /// <summary>
        /// Updates and existing record
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public async Task<TKey> Put<TData, TKey>(TData data)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<TKey>(DbFunctions.Put, Name, data);
        }

        /// <summary>
        /// Updates and existing record
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public async Task<TKey> Put<TData, TKey>(TData data, TKey key)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<TKey>(DbFunctions.Put, Name, data, key);
        }

        /// <summary>
        /// Deletes a record from the store based on the id
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task Delete<TKey>(TKey key) where TKey : notnull
        {
            await _idbDatabase.EnsureIsOpen();
            await _idbDatabase.CallJavascript(DbFunctions.Delete, Name, key);
        }

        /// <summary>
        /// Add an array of new record/object in one transaction to the specified store
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public async Task BatchAdd<TData>(TData[] data)
        {
            await _idbDatabase.EnsureIsOpen();
            await _idbDatabase.CallJavascript(DbFunctions.BatchAdd, Name, data);
        }

        /// <summary>
        /// Add an array of new record/object in one transaction to the specified store
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public async Task<TKey[]> BatchAdd<TData, TKey>(TData[] data)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<TKey[]>(DbFunctions.BatchAdd, Name, data);
        }

        /// <summary>
        /// Put an array of new record/object in one transaction to the specified store
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public async Task BatchPut<TData>(TData[] data)
        {
            await _idbDatabase.EnsureIsOpen();
            await _idbDatabase.CallJavascript(DbFunctions.BatchPut, Name, data);
        }

        /// <summary>
        /// Put an array of new record/object in one transaction to the specified store
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public async Task<TKey[]> BatchPut<TData, TKey>(TData[] data)
        {
            await _idbDatabase.EnsureIsOpen();
            return await _idbDatabase.CallJavascript<TKey[]>(DbFunctions.BatchPut, Name, data);
        }

        /// <summary>
        /// Delete multiple records from the store based on the id
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="TInput"></typeparam>
        /// <returns></returns>
        public async Task BatchDelete<TKey>(TKey[] ids)
        {
            await _idbDatabase.EnsureIsOpen();
            await _idbDatabase.CallJavascript(DbFunctions.BatchDelete, Name, ids);
        }

        /// <summary>
        /// Clears all of the records from a given store.
        /// </summary>
        /// <returns></returns>
        public async Task ClearStore()
        {
            await _idbDatabase.EnsureIsOpen();
            var result = await _idbDatabase.CallJavascript<string>(DbFunctions.ClearStore, Name);
        }
    }
}
