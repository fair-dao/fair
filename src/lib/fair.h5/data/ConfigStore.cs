using fair.extensions.shared.IndexedDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fair.extensions.shared.data
{


    public class ConfigStore : IDBObjectStore
    {
        public ConfigStore(IDBDatabase database) : base(database)
        {
            Name = "Config";
            KeyPath = "id";
            AutoIncrement = false;
        }
    }

}
