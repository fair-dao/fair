using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fair.extensions.shared.exs
{
    /// <summary>
    /// 已处理的异常，gensys异常基类,所有处理的异常必须由此类继承
    /// </summary>
    public class ProcessedException : System.Exception
    {

        public ProcessedException(string msg=null,Exception inner=null) : base(msg,inner)
        {
            msg = msg ?? "很抱歉，您的操作未成功";
            this.HResult = 0x10000;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="errCode">错误码</param>
        /// <param name="innerExcetpin"></param>
        public ProcessedException(string msg, int errCode, Exception innerExcetpin = null) : base(msg, innerExcetpin)
        {
            this.HResult = errCode;
        }

    }
}
