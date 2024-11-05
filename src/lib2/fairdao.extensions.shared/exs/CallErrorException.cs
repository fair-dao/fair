using fairdao.extensions.shared.entity;

namespace fairdao.extensions.shared.exs
{
    public class CallErrorException : ProcessedException
    {
        public CallErrorException(string msg) : base(msg)
        {

        }


    }

    /// <summary>
    /// 无权访问异常
    /// </summary>
    public class NoPowerException: ProcessedException
    {
        public NoPowerException(string msg): base(msg,(int)StateCode.没有权限)
        {

        }
        public NoPowerException() : base("您没有访问的权限", (int)StateCode.没有权限)
        {

        }
    }


    /// <summary>
    /// 无权访问异常
    /// </summary>
    public class NetworkException : ProcessedException
    {
        public NetworkException(string msg) : base(msg, (int)StateCode.网络已断开)
        {

        }
        public NetworkException() : base("网络已断开", (int)StateCode.网络已断开)
        {

        }
    }

}