using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace fairdao.extensions.shared
{
    /// <summary>
    /// 返回结果
    /// </summary>
    public class Result
    {

        /// <summary>
        /// 状态
        /// </summary>
        public string state { get; set; } = "ok";

        /// <summary>
        /// 指令
        /// </summary>
        public string? cmd { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        public string msg { get; set; } = "";

        /// <summary>
        /// 返回数据
        /// </summary>
        public object data { get; set; } = null;

        public bool IsOK()
        {
            return state == STATE_OK;
        }
        public static Result Err(string msg)
        {
            return new Result { state = STATE_ERR, msg = msg };
        }

        public static Result Err(string msg, object data)
        {
            return new Result { state = STATE_ERR, msg = msg, data = data };
        }


        /// <summary>
        /// 成功返回，并要求客户端跳转到指定页面
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Result OK_JMP(string url)
        {
            return Result.OK(data: new { url = url });
        }

        public static Result UserNoExist()
        {
            return new Result { state = STATE_ERR, msg = "账号不存在" };
        }


        public static Result NotLogin(string msg = "您未登录", object data = null)
        {
            return new Result { state = "notlogin", msg = msg, data = data };
        }


        /// <summary>
        /// 返回警告信息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>

        public static Result Warn(string msg = "", object data = null)
        {
            return new Result { state = STATE_WARN, msg = msg, data = data };
        }

        public static Result OK(string msg = "", object data = null)
        {
            return new Result { state = STATE_OK, msg = msg, data = data };
        }

        /// <summary>
        /// 操作失败
        /// </summary>
        /// <returns></returns>
        public static Result OPERR()
        {
            return new Result { state = STATE_OK, msg = "操作失败" };
        }

        /// <summary>
        /// 返回严重错误信息
        /// </summary>
        /// <returns></returns>
        public static Result SeriousErr()
        {
            return new Result { state = "err", msg = "操作失败" };
        }

        /// <summary>
        /// 无权操作
        /// </summary>
        /// <returns></returns>
        public static Result PowerErr()
        {
            return new Result { state = STATE_ERR_NOPOWER, msg = "无权操作" };
        }



        public const string STATE_OK = "ok";

        public const string STATE_ERR = "err";

        public const string STATE_ERR_NOPOWER = "nopower";

        public const string STATE_WARN = "warn";

        public const string STATE_NOTLOGIN = "notlogin";

        public const string MSG_SUCCESS = "操作成功";
        public const string MSG_FAIL = "操作失败";

    }

    public class Result<T> : Result
    {

        /// <summary>
        /// 返回数据
        /// </summary>
        public new T data { get; set; } = default(T);


        public static Result<T> OK(string msg = "", T data = default(T))
        {
            return new Result<T> { state = Result.STATE_OK, msg = msg, data = data };
        }


        public static Result<T> NotLogin(string msg = "您未登录", T data = default(T))
        {
            return new Result<T> { state = "notlogin", msg = msg, data = data };
        }


        public static new Result<T> Err(string msg = "")
        {
            return new Result<T> { state = Result.STATE_ERR, msg = msg };
        }


        /// <summary>
        /// 无权操作
        /// </summary>
        /// <returns></returns>
        public static new Result<T> PowerErr()
        {
            return new Result<T> { state = STATE_ERR_NOPOWER, msg = "无权操作" };
        }



        /// <summary>
        /// 执行指定操作
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Result<T> Do(Func<Result<T>> func)
        {
            if (func != null)
            {
                Result<T> r = func();
                return r;
            }
            throw new Exception("执行体不存在");

        }




    }


    public class Result<T1, T2> : Result<T1>
    {
        public T2 data2 { get; set; }

        public static Result<T1, T2> OK(string msg = "", T1 data = default(T1), T2 data2 = default(T2))
        {
            return new Result<T1, T2> { state = Result.STATE_OK, msg = msg, data = data, data2 = data2 };
        }


        public static Result<T1, T2> NotLogin(string msg = "您未登录")
        {
            return new Result<T1, T2> { state = "notlogin", msg = msg };
        }


        public static new Result<T1, T2> Err(string msg = "")
        {
            return new Result<T1, T2> { state = Result.STATE_ERR, msg = msg };
        }
    }
}
