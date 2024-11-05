using System;
using System.Collections.Generic;
using System.Linq;


namespace fairdao.extensions.shared.exs
{
    /// <summary>
    /// 密码出错
    /// </summary>
    public class PasswordException:Exception
    {
        public PasswordException(string msg = null, Exception inner = null) : base(msg, inner)
        {
            msg = msg ?? "密码错误";
            this.HResult = 0x10002;
        }
    }
}