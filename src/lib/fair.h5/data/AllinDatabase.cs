using fair.extensions.shared.IndexedDB;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fair.extensions.shared.data
{
    public class gensysDatabase : IDBDatabase
    {

        public ConfigStore ConfigStore { get; }

       

        public gensysDatabase(IJSRuntime jsRuntime) : base(jsRuntime)
        {
            Name = "gensys";
            Version = 1;
            ConfigStore = new ConfigStore(this);

        }
    }
}
