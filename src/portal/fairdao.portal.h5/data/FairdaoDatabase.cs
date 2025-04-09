using fairdao.extensions.shared.IndexedDB;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fairdao.extensions.shared.data
{
    public class FairdaoDatabase : IDBDatabase
    {

        public ConfigStore ConfigStore { get; }

       

        public FairdaoDatabase(IJSRuntime jsRuntime) : base(jsRuntime)
        {
            Name = "fairdao";
            Version = 1;
            ConfigStore = new ConfigStore(this);

        }
    }
}
