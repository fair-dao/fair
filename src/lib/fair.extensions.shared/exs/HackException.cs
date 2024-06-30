using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fair.extensions.shared.exs
{
    /// <summary>
    /// 攻击性访问异常
    /// </summary>
    public class HackException : ProcessedException
    {
    
        /// <summary>
        /// 攻击性异常构造
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="inner"></param>
        public HackException(string msg = null, Exception inner = null) : base(msg, inner)
        {
            msg = msg ?? "访问异常";
            this.HResult = 0x400000;
        }


    }
}
