using fairdao.extensions.shared;
using fairdao.extensions.shared.entity;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace fairdao.extensions.shared
{
    public class UIBase : ComponentBase, IAsyncDisposable
    {

        public RenderFragment RenderDialog { get; set; }

        [Inject]
        protected NavigationManager NavManager { get; set; }

        [Inject]
        protected fairdao.extensions.shared.localization.GensysLocaler Localer { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public IAPIHttpClient httpClient { get; set; }


        /// <summary>
        /// 返回地址
        /// </summary>

        [Parameter]
        public string BackUrl { get; set; }


        /// <summary>
        /// 提示信息
        /// </summary>
        [Parameter]
        public fairdao.extensions.shared.entity.AlertMessage AlertMessage { get; set; }


        [Parameter]
        public EventHandler<Exception> ExceptionHander { get; set; }
        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="data"></param>
        /// <param name="e"></param>
        public virtual void ThrowException(object data, Exception e)
        {
            ExceptionHander?.Invoke(data, e);
            // this.Toast(e.Message);
        }

        /// <summary>
        /// 获取连接网址
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public string GetLinkUrl(VCommpent commpent)
        {
            if (commpent.ComType== ComponentType.ThirdLink || commpent.ComType==ComponentType.IconMode )
            {
                return commpent.Link;
            }
            else
            {
                return $"/fairdao/load/{commpent.Id}";
            }
        }


        /// <summary>
        /// 获取连接网址
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public string GetLinkUrl(string link)
        {           
            if (link.IndexOf('/') >= 0)
            {
                return link;
            }
            else
            {
                return $"/fairdao/load/{link.Replace(".","-")}";
            }
        }


        /// <summary>
        /// 动态出现的组件（主要用于通知或，信息提醒） 
        /// </summary>
        public static System.Collections.Concurrent.ConcurrentDictionary<Guid, RenderFragment> Notifications { get; set; } = new System.Collections.Concurrent.ConcurrentDictionary<Guid, RenderFragment>();

        public static EventHandler NotificationsChanged;

        public void Nav(string url, bool forceload = false)
        {
            NavManager.NavigateTo(url, forceload);
        }

        /// <summary>
        /// 显示出错框
        /// </summary>
        /// <param name="title"></param>
        /// <param name="err"></param>
        /// <param name="showMillSeconds"></param>
        public void AddErrNotification(string title, string err, int showMillSeconds = 8000)
        {
            fairdao.extensions.shared.entity.AlertMessage msg = new AlertMessage();
            msg.ShowType = ShowTypes.Alert;
            msg.Title = title;
            msg.Type = AlertMessageType.Err;
            msg.Message = err;
            AddNotification(msg);
        }


        public void Toast(string msg, string level = "")
        {
            if (msg == null) return;
            level = level ?? "";
            JSRuntime.InvokeVoidAsync("fairdao.ui.Toast", msg, level);
        }


        /// <summary>
        /// 在指定位置弹出指定层
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Popup(string selector, double x, double y)
        {
            JSRuntime.InvokeVoidAsync("fairdao.ui.Popup", selector, x, y);
        }

        /// <summary>
        /// 显示出错框
        /// </summary>
        /// <param name="title"></param>
        /// <param name="err"></param>
        /// <param name="showMillSeconds"></param>
        public void Alert(string title, string msg, int showMillSeconds = 8000)
        {
            fairdao.extensions.shared.entity.AlertMessage msg1 = new AlertMessage();
            msg1.ShowType = ShowTypes.Alert;
            msg1.Title = title;
            msg1.Message = msg;
            AddNotification(msg1);
        }




        /// <summary>
        /// 显示出错框
        /// </summary>
        /// <param name="title"></param>
        /// <param name="err"></param>
        /// <param name="showMillSeconds"></param>
        public void Prompt(string title, string msg, Action<string> ok, string defaultValue = "")
        {
            fairdao.extensions.shared.entity.AlertMessage msg1 = new AlertMessage();
            msg1.ShowType = ShowTypes.Prompt;
            msg1.Title = title;
            msg1.Message = msg;
            msg1.Ok = ok;
            msg1.DefaultValue = default;
            AddNotification(msg1);
        }

        /// <summary>
        /// 显示出错框
        /// </summary>
        /// <param name="title"></param>
        /// <param name="err"></param>
        /// <param name="showMillSeconds"></param>
        public void Confirm(string title, string msg, Action yes, Action cancel = null, string btnTexts = null)
        {
            fairdao.extensions.shared.entity.AlertMessage msg1 = new AlertMessage();
            msg1.ShowType = ShowTypes.Confirm;
            msg1.Title = title;
            msg1.Message = msg;
            msg1.Yes = yes;
            msg1.ButtonTexts = btnTexts;
            if (cancel != null) msg1.Cancel = cancel;
            AddNotification(msg1);
        }




        public void AddNotification(fairdao.extensions.shared.entity.AlertMessage msg)
        {
            Guid guid = Guid.NewGuid();
            Console.WriteLine($"{msg.Message},{guid}");
            RenderFragment rf = new RenderFragment(builder =>
              {
                  //builder.OpenComponent(0, typeof(UNotificationMessage));
                  builder.AddAttribute(1, "Id", guid);
                  builder.AddAttribute(2, "NotiMessage", msg);
                  builder.CloseComponent();
              });
            Notifications.TryAdd(guid, rf);
            if (NotificationsChanged != null)
            {
                NotificationsChanged(msg, EventArgs.Empty);
            }

        }



        public virtual async Task GoBack(int pages = -1)
        {
            await JSRuntime.InvokeVoidAsync("history.go", pages);
        }
        public virtual async Task Back()
        {
            await JSRuntime.InvokeVoidAsync("history.back", -1);
        }


        protected async override void OnInitialized()
        {
            //创建对话框组件 

            AlertMessage = new AlertMessage();
            base.OnInitialized();
        }

        /// <summary>
        /// 附加属性
        /// </summary>
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> Attributes { get; set; }


        /// <summary>
        /// class属性内容
        /// </summary>
        public string AttrClass
        {
            get
            {
                if (Attributes != null && Attributes.ContainsKey("class"))
                {
                    return Attributes["class"].ToString();
                }
                return string.Empty;
            }
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {

            if (!firstRender)
            {
                if (AlertMessage != null && !string.IsNullOrWhiteSpace(AlertMessage.Message))
                {
                    string msg = AlertMessage.Message;
                    this.Toast(msg, AlertMessage.Type.ToString().ToLower());
                    AlertMessage.Message = string.Empty;

                }
                //JSRuntime.InvokeVoidAsync("com.runSelector", "app");
            }
            return base.OnAfterRenderAsync(firstRender);


        }





        /// <summary>
        /// 释放资源
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            //Console.WriteLine($"释放UIBase:{this.GetType()}: {this.navManager.Uri}");
            await DisposeAsync(disposing: true);
            GC.SuppressFinalize(this);

        }
        protected virtual ValueTask DisposeAsync(bool disposing)
        {
            return ValueTask.CompletedTask;
        }







    }
}
