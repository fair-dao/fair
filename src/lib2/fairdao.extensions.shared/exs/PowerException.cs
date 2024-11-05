using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fairdao.extensions.shared.exs
{

    /// <summary>
    /// 没有访问的权限
    /// </summary>
    public class PowerException:ProcessedException
    {

        public PowerException(string msg = null, Exception inner = null) : base(msg, inner)
        {
            msg = msg ?? "您没有访问的权限";
            this.HResult = 0x10001;
        }

    }
}
