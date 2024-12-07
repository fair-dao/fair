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
using fairdao.extensions.shared;
using fairdao.ui;
using System.Runtime.InteropServices;
using Microsoft.FluentUI.AspNetCore.Components;

namespace fairdao.extensions.shared
{
    public class ComBase : UIBase
    {

        public void ComponentClick(VCommpent commpent)
        {
            if (commpent.ComType == ComponentType.ThirdLink)
            {
                NavManager.NavigateTo(commpent.Link);
            }
            else
            {
                NavManager.NavigateTo($"/fairdao/load/{commpent.Id}");
            }

        }

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
        public virtual string CurPlugId { get; }

        /// <summary>
        /// 页面加载中
        /// </summary>
        public virtual bool? Loaded { get; set; }


        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public IToastService ToastService { get; set; }
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



    
    


        /// <summary>
        /// 包装处理结果，自动处理异常信息
        /// </summary>
        ///<param name="action">动作</param>
        /// <returns></returns>
        public async Task WraperFromResult<T>(string url, object? data=null, HttpMethod? httpMethod= null,Action<T> okAction = null,string token=null)
        {
            this.Loaded = false;
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.WithBearer(token);
                }
                
                await httpClient.SubmitResult<T>(url, data, httpMethod).ContinueWith(preTask =>
                    {
                        this.Loaded = true;
                        if (preTask.Exception?.InnerException == null)
                        {
                            if (okAction != null) okAction(preTask.Result);
                        }
                        else throw preTask.Exception?.InnerException;
                    });
            }catch(Exception e)
            {
                this.Loaded = true;
                this.DialogService.ShowErrorAsync(e.Message);
                this.StateHasChanged();
            }
          
        }

    }

    
}
