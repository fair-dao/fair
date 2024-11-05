using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fairdao.extensions.shared.exs
{

    /// <summary>
    /// 重要异常，需要日志记录
    /// </summary>
    public class ImportantException : Exception
    {
        /// <summary>
        /// 异常构造函数
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="innerException"></param>
        public ImportantException(string msg,Exception innerException) : base(msg, innerException)
        {

        }
        public ImportantException(string msg) : base(msg)
        {

        }
    }
}