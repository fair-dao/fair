using fairdao.extensions.shared.IndexedDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fairdao.extensions.shared.data
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
