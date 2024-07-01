using fairdao.extensions.shared.entity;

namespace fairdao.extensions.shared.exs
{

    /// <summary>
    /// API处理异常
    /// </summary>
    public class APICodeException:ProcessedException
    {
        public APICodeException(string msg) : base(msg)
        {
            
            this.HResult = (int)StateCode.未知错误;
        }

     

        public StateCode StateCode;

        public string Token;
        public APICodeException(StateCode code,string token, string msg) :base(msg)
        {
            this.StateCode = code;
            this.Token = token;
        }

        public APICodeException(StateCode code, string msg=""):base(msg) 
        {
            this.StateCode = code;
            this.HResult = (int)code;


        }

    }
}
