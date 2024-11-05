using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace fairdao.extensions.shared.IndexedDB
{
    /// <summary>
    /// Provides functionality for accessing IndexedDB from Blazor application
    /// </summary>
    public abstract class IDBDatabase
    {
        private struct DbFunctions
        {
            public const string Open = "open";
            public const string DeleteDatabase = "deleteDatabase";
            public const string GetDbSchema = "getDbSchema";
        }

        private readonly IJSRuntime _jsRuntime;
        private const string InteropPrefix = "BlazorIndexedDbJs.IDBManager";
        private bool _isOpen;
        private List<IDBObjectStore> _objectStores = new List<IDBObjectStore>();

        public string Name { get; init; } = "";
        public int Version { get; set; }
        public List<IDBObjectStore> ObjectStores => _objectStores;

        public List<string> ObjectStoreNames
        {
            get
            {
                return _objectStores.Select(s => s.Name).ToList();
            }
        }

        public IDBDatabase(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Opens the IndexedDB defined in the DbDatabase. Under the covers will create the database if it does not exist
        /// and create the stores defined in DbDatabase.
        /// </summary>
        /// <returns></returns>
        public async Task Open()
        {
            var dbdef = IDBSchema.GetDatabaseDef(Name, Version, _objectStores);
            await CallJavascript(DbFunctions.Open, dbdef);
            _isOpen = true;
        }

        /// <summary>
        /// Deletes the database corresponding to the dbName passed in
        /// </summary>
        /// <param name="dbName">The name of database to delete</param>
        /// <returns></returns>
        public async Task DeleteDatabase()
        {
            await CallJavascript(DbFunctions.DeleteDatabase, Name);
            _isOpen = false;
        }

        /// <summary>
        /// get ObjectSore by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDBObjectStore ObjectStore(string storeName)
        {
            var store = _objectStores.Find(s => s.Name == storeName);
            if (store == null)
            {
                throw new IDBNotFoundError($"Store {storeName} does not exists");
            }
            return store;
        }

        /// <summary>
        /// Load dabatabase schema from databaseName
        /// </summary>
        /// <returns></returns>
        public async Task LoadSchemaFromDatabase(string databaseName)
        {
            var result = await CallJavascript<IDBSchema.IDBDatabaseDef>(DbFunctions.GetDbSchema, databaseName);

            Version = result.Version;

            _objectStores.Clear();
            foreach (var item in result.ObjectStores)
            {
                var store = new IDBObjectStore(this)
                {
                    Name = item.Name,
                    KeyPath = item.KeyPath,
                    AutoIncrement = item.AutoIncrement
                };
                _objectStores.Add(store);
            }
        }

        /// <summary>
        /// This function provides the means to add a store to an existing database,
        /// </summary>
        /// <param name="objectStore"></param>
        /// <returns></returns>
        public async Task CreateObjectStore(IDBObjectStore objectStore)
        {
            await EnsureIsOpen();
            if (objectStore == null)
            {
                return;
            }

            if (_objectStores.Any(s => s.Name == objectStore.Name))
            {
                return;
            }
            _objectStores.Add(objectStore);
            Version += 1;
            await Open();
        }

        public async Task ConsoleLog(params object[] args)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("console.log", args);
            }
            catch (JSException e)
            {
                throw new IDBException(e.Message);
            }
        }

        public async Task CallJavascript(string functionName, params object[] args)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync($"{InteropPrefix}.{functionName}", args);
            }
            catch (JSException e)
            {
                throw new IDBException(e.Message);
            }
        }

        public async Task<TResult> CallJavascript<TResult>(string functionName, params object[] args)
        {
            try
            {
                return await _jsRuntime.InvokeAsync<TResult>($"{InteropPrefix}.{functionName}", args);
            }
            catch (JSException e)
            {
                throw new IDBException(e.Message);
            }
        }

        public async Task EnsureIsOpen()
        {
            if (!_isOpen) await Open();
        }
    }
}
