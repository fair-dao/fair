using fairdao.extensions.shared;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace fairdao.extensions.shared
{

    /// <summary>
    /// 数据提交接口
    /// </summary>
    public interface IAPIHttpClient
    {
        Task<T> ToSend<T>(string url, HttpContent content = null, HttpMethod method = null);
        Task<T> SendResult<T>(string url, HttpContent content = null, HttpMethod? method = null);
      //  IAPIHttpClient FlushToken();
        Task<string> GetString(string url);

        /// <summary>
        /// 处理Result
        /// </summary>
        /// <param name="ExceptionHander"></param>
        IAPIHttpClient With(EventHandler<Result> resultHander);


        /// <summary>
        /// 携带token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>

        IAPIHttpClient WithBearer(string token);

    }
}