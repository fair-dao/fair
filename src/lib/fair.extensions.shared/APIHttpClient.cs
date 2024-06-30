using fair.extensions.shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace fair.extensions.shared
{
    public class APIHttpClient : HttpClient, IAPIHttpClient
    {

        private fair.extensions.shared.SysHelper helper;

        EventHandler<Exception> ExceptionHander;

        /// <summary>
        /// 结果处理
        /// </summary>
        EventHandler<Result> ResultHander;

        public APIHttpClient(shared.SysHelper helper) :base()
        {
            this.Timeout = TimeSpan.FromSeconds(8);
            this.helper = helper;
            if (!string.IsNullOrEmpty(helper.ApiUrl))
            {
                this.BaseAddress = new Uri(helper.ApiUrl);
            }

        }


        public IAPIHttpClient With(EventHandler<Exception> e)
        {
            this.ExceptionHander = e;
            return this;
        }



        public readonly object lockobj = new object();

        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            return base.SendAsync(request, cancellationToken);

        }


        ///// <summary>
        ///// 调用API方法
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="url"></param>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //public async Task<Result> PostResult(string url, string data = "")
        //{
        //    var content = new StringContent(data);
        //    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
        //    return await PostResult(url, content);


        //}



        ///// <summary>
        ///// 调用API方法
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="url"></param>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //public async Task<Result> PostResult(string url, HttpContent content)
        //{
        //    return await this.Submit<fair.extensions.shared.Result>(url, content, HttpMethod.Post);

        //}



        ///// <summary>
        ///// 调用API方法
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="url"></param>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //public Task<T> PostResult<T>(string url, string data = "")
        //{

        //    var content = new StringContent(data);
        //    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
        //    return SubmitResult<T>(url, content, HttpMethod.Post);

        //}


        ///// <summary>
        ///// 调用API方法
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="url"></param>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //public async Task<T> PostResult<T>(string url, HttpContent content)
        //{
        //    return await SubmitResult<T>(url, content, HttpMethod.Post);
        //}


        public Task<string> GetString(string url)
        {
            return this.GetStringAsync(url);
        }

        public IAPIHttpClient With(EventHandler<Result> resultHandler)
        {
            this.ResultHander = resultHandler;
            return this;
        }

        /// <summary>
        /// 提交并返回数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public Task<T> SendResult<T>(string url, HttpContent content = null, HttpMethod? method = null)
        {
            return this.ToSend<Result<T>>(url, content, method).ContinueWith<T>(r =>
            {
                Exception e = null;
                Result<T> o = null;

                if (r.Exception == null)
                {

                  
                    o = r.Result;
                    if (o != null)
                    {

                        ResultHander?.Invoke(o, o);
                        if (o.state == Result.STATE_OK)
                        {
                            return o.data;
                        }


                        if (o.state == "notlogin") //未登录
                        {
                            e = new fair.extensions.shared.exs.NoLoginException(o.msg);
                        }
                        else if (o.state == Result.STATE_ERR_NOPOWER) //无权访问
                        {
                            e = new fair.extensions.shared.exs.NoPowerException(o.msg);
                        }
                        else if (!string.IsNullOrEmpty(o.msg))
                        {
                            e = new Exception(o.msg);
                        }
                        else e= new Exception($"数据获取失败{url}");
                    }
                    e = e ?? new Exception("调用失败");
                    this.ExceptionHander?.Invoke(o, e);
                    throw e;
                }
                else
                {
                    throw r.Exception;
                }

            




            });
        }

        /// <summary>
        /// 提交表单 
        /// </summary>
        /// <typeparam name="T">返回数据类型</typeparam>
        /// <param name="c">http客户端</param>
        /// <param name="url">提交地址</param>
        /// <param name="content">提交的数据</param>
        /// <param name="method">提交的方法</param>
        /// <returns></returns>
        public async Task<T> ToSend<T>(string url, HttpContent content = null, HttpMethod? method = null)
        {
            Console.WriteLine($"ToSend {url},{this.DefaultRequestHeaders.Authorization}");

            if (content == null)
            {
                content = new StringContent(string.Empty);
            }
            if (method == null)
            {
                method = HttpMethod.Post;
            }

            Uri uri = this.BaseAddress == null ? new Uri(url) : new Uri(this.BaseAddress, url);


            var httpRequestMessage = new HttpRequestMessage
            {
                Method = method,
                RequestUri = uri,
                Content = content
                // Content = new StringContent(data.ToJson(), Encoding.UTF8, "application/json")
            };
            //Console.WriteLine($"httclient:{c},Form地址:{url}");
            //System.Threading.CancellationToken c = new System.Threading.CancellationToken();
            try
            {
                //c.Timeout = TimeSpan.FromSeconds(1);

                HttpResponseMessage r = await this.SendAsync(httpRequestMessage);
                string back = await r.Content.ReadAsStringAsync();
                if (r.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"{url}\r\nbackdata:   {back}");
                    T o = System.Text.Json.JsonSerializer.Deserialize<T>(back, new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = null });
                    return o;
                }
                else
                {
                    switch (r.StatusCode)
                    {
                        case System.Net.HttpStatusCode.NotFound:
                            throw new fair.extensions.shared.exs.APICodeException(fair.extensions.shared.entity.StateCode.请求的资源不存在);

                        default:
                            throw new fair.extensions.shared.exs.APICodeException(fair.extensions.shared.entity.StateCode.操作失败, "出错：状态码: " + r.StatusCode);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("url:{0},ex:{1}", url, e);
                ExceptionHander?.Invoke(url, e);
                throw e;

            }
        }

    }
}
