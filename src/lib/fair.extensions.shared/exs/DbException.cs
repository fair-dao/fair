using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fair.extensions.shared.exs
{

    /// <summary>
    /// 数据操作异常(0x100000)
    /// </summary>
    public class DbException : ProcessedException {

        public DbException(string msg = null, Exception inner = null) : base(msg, inner)
        {
            msg = msg ?? "数据访问异常";
            this.HResult = 0x100000;
        }
    }
}
