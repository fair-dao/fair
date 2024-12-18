using clientApp.Data;
using fairdao.extensions.shared;
using fairdao.extensions.shared.entity;
using fairdao.portal.shared;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.Logging;
using System.Reflection;

using Microsoft.Maui.Platform;
using fairdao.portal;
using Microsoft.FluentUI.AspNetCore.Components;



#if ANDROID
using AndroidX.Activity;
#endif


namespace Microsoft.Extensions.DependencyInjection
{
    public static class FairExtensions
    {


        public static readonly Env Env = new();


        public static async Task<MauiApp> RunFairHost(
          this MauiAppBuilder builder, params fairdao.extensions.shared.ExtenderBase[]? extenders)
        {
            SysHelper.AppStartTime = DateTime.Now;


#if ANDROID
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<IBlazorWebView, MauiBlazorWebViewHandler>();
            });
#endif
            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif


            Console.WriteLine($"开始注入服务...({fairdao.extensions.shared.SysHelper.RunedSpanTime}毫秒)");
            #region 加入数据存储服务

            builder.Services.AddSingleton<IDataStore, SQLiteDataStore>();

            #endregion


            //配置环境


            builder.Services.AddSingleton<Env>(Env);
            builder.Services.AddSingleton<SysHelper, AppHelper>();
            builder.Services.AddFluentUIComponents();
            Configure.ConfigureServices(builder.Services,extenders);

            Console.WriteLine($"数据服务、配置环境、辅助服务已注入，开始初始化各服务...({SysHelper.RunedSpanTime}毫秒)");

            var app = builder.Build();
            await Configure.ConfigureProviders(app.Services);
            Console.WriteLine($"所有服务初始化已完成.({SysHelper.RunedSpanTime}毫秒)");

            return app;

        }
        public static void InitBlazorWebView(this BlazorWebView webView)
        {
            webView.RootComponents[0].ComponentType = typeof(fairdao.extensions.shared.Routes);
            webView.BlazorWebViewInitialized += (sender, e) =>
            {

#if ANDROID

                var a = e.WebView.Context.GetActivity();
                if (a is FairdaoMainActivity)
                {
                    FairdaoMainActivity mainActivity = (FairdaoMainActivity)a;
                    mainActivity.SetWebView(e.WebView);
                }
                //if (e.WebView.Context?.GetActivty() is not ComponentActivity activity)
                //{
                //    throw new InvalidOperationException($"The permission-managing WebChromeClient requires that the current activity be a '{nameof(ComponentActivity)}'.");
                //}

#endif
            };
            webView.BlazorWebViewInitializing += (sender, e) =>
            {

#if IOS || MACCATALYST
            e.Configuration.AllowsInlineMediaPlayback = true;
            e.Configuration.MediaTypesRequiringUserActionForPlayback = WebKit.WKAudiovisualMediaTypes.None;
#endif
            };
        }

    }
}
