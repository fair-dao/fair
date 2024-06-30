using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fair.extensions.shared.exs
{

    /// <summary>
    /// 登录异常(HResult:0x10000001)
    /// </summary>
    public class NoLoginException :exs.ProcessedException
    {
        public NoLoginException(string msg = null, Exception inner = null) : base(msg, inner)
        {
            msg = msg ?? "您未登录";
            this.HResult = 0x10003;
        }

    }
}
