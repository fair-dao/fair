using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Rendering;
using System.Net.Http;
using Microsoft.JSInterop;
using fairdao.extensions.shared.entity;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using BulmaRazor.Components;
using fairdao.extensions.shared;
using fairdao.ui;
using System.Runtime.InteropServices;

namespace fairdao.extensions.shared
{
    public class ComBase : fairdao.ui.UIBase
    {

        public bool IsManageSysUser
        {
            get
            {
                return NavManager.Uri.IndexOf("/suser-") > 0;
            }
        }

        /// <summary>
        /// 当前插件Id(在插件服务端data\app.json文件里查看)
        /// </summary>
        public virtual string CurPlugId{ get;}

        /// <summary>
        /// 页面加载中
        /// </summary>
        public virtual bool? Loaded { get; set; }


 

    /// <summary>
    /// 日志工厂
    /// </summary>
    [Inject]
        protected ILoggerFactory LoggerFactory { get; set; }

        private ILogger logger;

        /// <summary>
        /// 日志记录器
        /// </summary>
        public ILogger Logger
        {
            get
            {
                if (logger == null)
                {
                    logger = LoggerFactory.CreateLogger(this.GetType().Name);

                }
                return logger;

            }
        }


        [Inject]
        protected fairdao.extensions.shared.SysHelper sysHelper { get; set; }


      


        public override void ThrowException(object data, Exception e)
        {
     
            if (e is fairdao.extensions.shared.exs.NoPowerException) //无权访问
            {
                this.InvokeAsync(() => { this.Toast(e.Message); });
                GoBack();
                return;
            }
            if (e is System.Net.Http.HttpRequestException)
            {
                this.InvokeAsync(() =>
                {
                    this.Toast("服务器开小差了~^~");
                });
                return;

            }

            //if (e is fairdao.extensions.shared.exs.NoLoginException)
            //{
            //    sysHelper.Logout().ContinueWith(r =>
            //    {
            //        navManager.NavigateTo("/");
            //    });
            //    return;

            //}
            var apiException = e as fairdao.extensions.shared.exs.APICodeException;
            if (apiException != null)
            {
                Console.WriteLine(apiException.Message);
                this.InvokeAsync(() =>
                {
                    this.Toast(Localer[apiException.StateCode.ToString()]);
                });
                return;
            }
            if (e is System.Net.WebException)
            {
                this.InvokeAsync(() =>
                {
                    this.Toast("无法连接到网络，请稍候再试");
                });
                return;
            }
            else

                this.InvokeAsync(() => { this.Toast(e.Message); });
        }





        /// <summary>
        /// 包装处理结果，自动处理异常信息
        /// </summary>
        ///<param name="okAction">成功返回后调用的方法</param>
        /// <returns></returns>
        public void ProcessResult(Result result, Action okAction = null)
        {

            if (result == null)
            {
                return;
            }
            if (result.state == Result.STATE_OK)
            {
                if (!string.IsNullOrEmpty(result.msg))
                {
                    this.Alert("", result.msg);
                }
                if (okAction != null) okAction();
            }
            else
            {
                this.AddErrNotification(Localer["错误"], result.msg);
            }

        }





        /// <summary>
        /// 包装处理结果，自动处理异常信息
        /// </summary>
        ///<param name="action">动作</param>
        /// <returns></returns>
        public void WraperException(Func<Result> funResult)
        {

            try
            {
                var result = funResult();
                if (result.state != Result.STATE_OK)
                {
                    if (result.state == Result.STATE_NOTLOGIN)
                    {
                        throw new fairdao.extensions.shared.exs.NoLoginException();
                    }
                    throw new fairdao.extensions.shared.exs.CallErrorException(result.msg);
                }
            }
            catch (fairdao.extensions.shared.exs.ProcessedException e) //无权访问
            {
                this.AddErrNotification("", Localer["无权访问"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                this.AddErrNotification("", ex.Message);
                return;
            }

        }




        /// <summary>
        /// 包装处理结果，自动处理异常信息
        /// </summary>
        ///<param name="action">动作</param>
        /// <returns></returns>
        public void WraperAction(Task action)
        {

            try
            {
                action.Start();
            }
            catch (fairdao.extensions.shared.exs.ProcessedException e) //无权访问
            {
                this.AddErrNotification("", Localer["无权访问"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                this.AddErrNotification("", ex.Message);
                return;
            }

        }

        public void WraperAction(Task<Action> taskAction)
        {

            try
            {
                taskAction.Wait();
            }
            catch (fairdao.extensions.shared.exs.ProcessedException e) //无权访问
            {
                this.AddErrNotification("", Localer["无权访问"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                this.AddErrNotification("", ex.Message);
                return;
            }

        }

        /// <summary>
        /// 包装处理结果，自动处理异常信息
        /// </summary>
        ///<param name="action">动作</param>
        /// <returns></returns>
        public async Task WraperFromResult<T>(string url, object data, HttpMethod httpMethod, string format = "form", Action<T> okAction = null)
        {
            string str = null;
            if (data != null)
            {
                str = System.Text.Json.JsonSerializer.Serialize(data);
            }
            str ??= "";
            await WraperFromResult<T>(url, str, httpMethod, format, okAction);

        }



        /// <summary>
        /// 包装处理结果，自动处理异常信息
        /// </summary>
        ///<param name="action">动作</param>
        /// <returns></returns>
        public Task WraperFromResult<T>(string url, string data, HttpMethod httpMethod, string format = "form", Action<T> okAction = null)
        {
            this.Loaded = false;

            return httpClient.With(this.ThrowException).SubmitResult<T>(url, data, httpMethod).ContinueWith(preTask =>
                { 
                    this.Loaded= true;
                    if (preTask.Exception?.InnerException == null)
                    {
                        if (okAction != null) okAction(preTask.Result);

                    }
                });

        }



        /// <summary>
        /// 包装处理结果，自动处理异常信息
        /// </summary>
        ///<param name="action">动作</param>
        /// <returns></returns>
        public Task WraperFromResult(string url, string data, HttpMethod httpMethod, string format = "form", Action okAction = null)
        {

            this.Loaded = false;
            return httpClient.With(this.ThrowException).SubmitResult<string>(url, data, httpMethod).ContinueWith(r =>
            {
                this.Loaded = true;
                if (r.Exception?.InnerException == null)
                {
                    if (okAction != null)
                    {
                        okAction();
                        return;
                    }
                }

            });


        }
    }
}
