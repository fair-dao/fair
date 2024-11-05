using System;
using System.Collections.Generic;

namespace fairdao.extensions.shared.IndexedDB
{
    public static class IDBSchema
    {
        public class IDBDatabaseDef
        {
            public string Name { get; set; } = "";
            public int Version { get; set; }
            public List<IDBObjectStoreDef> ObjectStores { get; set; } = new List<IDBObjectStoreDef>();
        }

        public class IDBObjectStoreDef
        {
            public string Name { get; set; } = "";
            public string KeyPath { get; set; }
            public bool AutoIncrement { get; set; }
            public List<IDBIndexDef> Indexes { get; set; } = new List<IDBIndexDef>();
        }
        public class IDBIndexDef
        {
            public string Name { get; set; } = "";
            public string KeyPath { get; set; }
            public bool MultiEntry { get; set; }
            public bool Unique { get; set; }
        }

        public static IDBDatabaseDef GetDatabaseDef(string name, int version, List<IDBObjectStore> objectStores)
        {
            var def = new IDBDatabaseDef();

            def.Name = name;
            def.Version = version;

            foreach (var store in objectStores)
            {
                var obj = new IDBObjectStoreDef();

                obj.Name = store.Name;
                obj.KeyPath = store.KeyPath;
                obj.AutoIncrement = store.AutoIncrement;

                foreach (var storeIndex in store.Indexes)
                {
                    var idx = new IDBIndexDef()
                    {
                        Name = storeIndex.Name,
                        KeyPath = storeIndex.KeyPath,
                        MultiEntry = storeIndex.MultiEntry,
                        Unique = storeIndex.Unique
                    };
                    obj.Indexes.Add(idx);
                }

                def.ObjectStores.Add(obj);
            }

            return def;
        }
    }
}