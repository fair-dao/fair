using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;
using Microsoft.Maui.Platform;
using WebView = Android.Webkit.WebView;
using Graphics = Android.Graphics;
using AndroidApp = Android.App;
using AndroidNet = Android.Net;
using AndroidOS = Android.OS;
using Android.Content.Res;
using Microsoft.AspNetCore.Components.WebView.Maui;

namespace fairdao.portal
{
   public class FairdaoMainActivity : MauiAppCompatActivity, IDownloadListener
    {
        public IValueCallback mUploadMessage;
        private const string TAG = "MainActivity";

        private const int FILECHOOSER_RESULTCODE = 1;

        private WebView webview;
        public string UserAgent;
        string WebViewVer;
        /// <summary>
        /// Get the name of the device
        /// </summary>
        public string DeviceInfo
        {
            get
            {
                return $"{Build.Manufacturer},{Build.Model},{Build.Brand},Helper.ClientVer,OS:{Build.VERSION.Release},SDK:{((byte)Build.VERSION.SdkInt)},webview:{WebViewVer}";
            }
        }


        protected override void AttachBaseContext(Context @base)
        {
            Configuration config = new Configuration();
            config = @base.Resources.Configuration;
            config.FontScale = 1.0f;
            Context context = @base.CreateConfigurationContext(config);
            base.AttachBaseContext(@base);
        }




        public void SetWebView(object view)
        {
            webview = view as WebView;

            webview.Settings.MediaPlaybackRequiresUserGesture = false;
            curChooserWebChromeClient = new FileChooserWebChromeClient((uploadMsg, acceptType, capture) =>
            {
                mUploadMessage = uploadMsg;
                var i = new Intent(Intent.ActionGetContent);
                i.AddCategory(Intent.CategoryOpenable);
                i.SetType("image/*");
                StartActivityForResult(Intent.CreateChooser(i, "File Chooser"), FILECHOOSER_RESULTCODE);
            }, this);

            webview.SetDownloadListener(this);

            webview.SetWebChromeClient(curChooserWebChromeClient);
            webview.Settings.JavaScriptEnabled = true;

            webview.AddJavascriptInterface(this, "mauiplug");//添加js接口,绑定js里，window的mauiplug对象，可以用window.mauiplug调用本页函数

            //e.WebView.Settings.JavaScriptEnabled = true;
            //e.WebView.Settings.AllowFileAccess = true;
            //e.WebView.Settings.MediaPlaybackRequiresUserGesture = false;
            //e.WebView.Settings.SetGeolocationEnabled(true);
            //e.WebView.Settings.SetGeolocationDatabasePath(e.WebView.Context?.FilesDir?.Path);
            //e.WebView.SetWebChromeClient(new PermissionManagingBlazorWebChromeClient(e.WebView.WebChromeClient!, activity));

        }

        /// <summary>
        /// MauiPlug 初始化接口 给浏览器的js调用
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="paras">参数信息</param>
        /// <returns></returns>

        [Export("CallMauiPlug")]
        [JavascriptInterface]
        public Java.Lang.String CallMauiPlug(Java.Lang.String cmd, Java.Lang.String paras)
        {
            try
            {
                //参数表
                string[] paramList = null;
                if (paras != null && paras.Length() > 0)
                {
                    try
                    {
                        paramList = System.Text.Json.JsonSerializer.Deserialize<string[]>(paras.ToString());
                    }
                    catch { }
                }


                switch (cmd.ToString())
                {


                    case "message":
                        break;
                    case "device":
                        return new Java.Lang.String($"{DeviceInfo}");
                    case "bind":
                        if (paramList.Length < 3) return null;
                        string userId = paramList[0];
                        string tokenId = paramList[1];
                        string apiAddress = paramList[2];
                        ////设置参数并绑定设备             
                        //if (Helper.APIConfig?.Address != apiAddress)
                        //{
                        //    var config = new APIConfig();
                        //    config.Address = apiAddress;
                        //    Helper.APIConfig = config;
                        //}
                        //if (Helper.CurUser?.Token != tokenId)
                        //{
                        //    //获取TpnsId
                        //    string devToken = XGPushConfig.GetToken(this);

                        //    Task.Run(async () =>
                        //    {

                        //        using (HttpClient httpClient = new HttpClient())
                        //        {
                        //            httpClient.BaseAddress = new Uri(apiAddress);
                        //            var result = await httpClient.PostForm<Result<UserInfo>>(Path.Combine(apiAddress, "fairdao_user/BindDevice"), $"userId={userId}&tokenId={tokenId}&dev={DeviceInfo}&pushToken={devToken}");

                        //            if (result.state == "ok")
                        //            {
                        //                Helper.CurUser = result.data;
                        //            }
                        //        }

                        //    });

                        //}

                        break;
                    case "unbind":
                        break;

                }
                return new Java.Lang.String("ok");
            }
            catch
            {
                return new Java.Lang.String("error");
            }


        }

        public string DataPath;

        private string configFile;

        FileChooserWebChromeClient curChooserWebChromeClient;

        public FairdaoMainActivity() : base()
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} 对象初始化...");

        }



        protected override void OnCreate(Bundle savedInstanceState)
        {


            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} 页面开始创建");
            FairExtensions.Env.Manufacturer = Build.Manufacturer;
            FairExtensions.Env.DeviceId = Build.Id;
            FairExtensions.Env.Model = Build.Model;

            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            st.Start();

            //全屏，即隐藏状态栏，时间、信号这些也不可见
            // Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

            //半透明 任务栏
            //Window.SetFlags(WindowManagerFlags.TranslucentStatus,WindowManagerFlags.TranslucentStatus);

            ////全透明任务栏
            //Window.SetFlags(WindowManagerFlags.TranslucentNavigation, WindowManagerFlags.TranslucentNavigation);
            ////设置状态栏、导航栏色颜色为透明
            //Window.SetStatusBarColor(Graphics.Color.Transparent);
            //Window.SetNavigationBarColor(Graphics.Color.Transparent);



            base.OnCreate(savedInstanceState);
            //强制使用竖屏
            this.RequestedOrientation = ScreenOrientation.UserPortrait;

            try
            {

                var window = this.GetWindow();
                this.Window.AddFlags(WindowManagerFlags.KeepScreenOn);

                //UserAgent = webview.Settings.UserAgentString;
                //if (!string.IsNullOrEmpty(UserAgent))
                //{
                //    Match mc = Regex.Match(UserAgent, @"Chrome/([\d\.]{1,30})\s", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
                //    if (mc.Success)
                //    {
                //        WebViewVer = mc.Groups[1].Value;
                //    }
                //}

            }
            catch (Exception ex)
            {

            }
            ////上传机器信息

            //Task.Run(async () =>
            //{
            //    try
            //    {

            //        var result = await Helper.UploadLog(this.ClientConfig.ApiServers.FirstOrDefault(m => m.Id == this.ClientConfig.CurApiServerId).Address, DeviceInfo);
            //    }
            //    catch { }
            //});






            string[] perms = new String[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.RecordAudio, Manifest.Permission.Camera, Manifest.Permission.CallPhone };
            List<String> unPerms = new List<string>();
            foreach (string p in perms)
            {
                if (CheckSelfPermission(p) != Permission.Granted)
                {
                    unPerms.Add(p);
                }
            }
            if (unPerms.Count > 0)
            {
                RequestPermissions(unPerms.ToArray(), 1);
            }



            ////bool needUpdate = true;
            //int sdk = ((byte)Build.VERSION.SdkInt);

            //if (sdk < 29)
            //{
            //    if (WebViewVer != null)
            //    {
            //        int inx = WebViewVer.IndexOf('.');
            //        if (inx > 0)
            //        {
            //            int ver = Int32.Parse(WebViewVer.Substring(0, inx));
            //            if (ver >= 80)
            //            {
            //                needUpdate = false;
            //            }
            //        }

            //    }

            //}
            //else needUpdate = false;

            //if (needUpdate)
            //{
            //    var apiServer = ClientConfig.ApiServers.FirstOrDefault(m => m.Id == ClientConfig.CurApiServerId);
            //    string domain = "";
            //    if (!string.IsNullOrEmpty(apiServer?.Address))
            //    {
            //        domain = new Uri(apiServer.Address).Host;
            //    }
            //    webview.LoadUrl($"https://0.0.0.0/update.html?domain={domain}&viewver={WebViewVer}");
            //}
            st.Stop();

            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} 初始化花费：{st.ElapsedMilliseconds}毫秒,到此已花费{fairdao.extensions.shared.SysHelper.RunedSpanTime} ms");



        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        int hitTimes = 0;
        public override void OnBackPressed()
        {
            if (webview != null)
            {
                if (webview.CanGoBack())
                {
                    hitTimes = 0;
                    webview.GoBack();

                }
                else
                {
                    hitTimes++;
                    if (hitTimes > 1)
                    {
                        System.Environment.Exit(0);
                    }
                    else
                    {
                        Task.Run(async () =>
                        {
                            await Task.Delay(1000);
                            hitTimes = 0;
                        });
                        Toast.MakeText(this, "快速按返回键两次退出", ToastLength.Short).Show();
                        return;
                    }


                }
            }


            base.OnBackPressed();
        }


        protected override void OnActivityResult(int requestCode, AndroidApp.Result resultCode, Intent intent)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, intent);
                if (resultCode == AndroidApp.Result.Ok && mUploadMessage != null) //确认了
                {
                    switch (requestCode) //根据传过来的请求值判断是属于那种情况
                    {
                        case 3: //拍照
                            AndroidNet.Uri[] results = new AndroidNet.Uri[] { curChooserWebChromeClient.photoURI };
                            mUploadMessage.OnReceiveValue(results);
                            curChooserWebChromeClient.photoURI = null;
                            break;
                        case 2: //录制
                        case FILECHOOSER_RESULTCODE: //选择文件 1
                            AndroidNet.Uri data = intent?.Data;
                            if (data == null) mUploadMessage.OnReceiveValue(null);
                            else
                            {
                                mUploadMessage.OnReceiveValue(new AndroidNet.Uri[] { data });
                            }
                            break;
                        default: //选择文件等
                            mUploadMessage?.OnReceiveValue(null);
                            break;
                    }

                }
                else
                {
                    mUploadMessage?.OnReceiveValue(null);

                }

            }
            catch (Exception ex)
            {
                mUploadMessage.OnReceiveValue(null);
            }
            finally
            {
                mUploadMessage = null;
            }
        }


        public void OnDownloadStart(string url, string userAgent, string contentDisposition, string mimetype, long contentLength)
        {

            Intent intent = new Intent(Intent.ActionView);
            intent.AddCategory(Intent.CategoryBrowsable);
            intent.SetData(AndroidNet.Uri.Parse(url));
            StartActivity(intent);
        }
    }

    partial class FileChooserWebChromeClient : WebChromeClient
    {
        FairdaoMainActivity mainActivity;
        Action<IValueCallback, Java.Lang.String, Java.Lang.String> callback;


        public FileChooserWebChromeClient(Action<IValueCallback, Java.Lang.String, Java.Lang.String> callback, FairdaoMainActivity mainActivity)
        {
            this.callback = callback;
            this.mainActivity = mainActivity;
        }

        public AndroidNet.Uri photoURI;

        /**
    * 拍照
*/
        private void takePhoto()
        {
            photoURI = null;
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            var documentsDirectry = this.mainActivity.GetExternalFilesDir(AndroidOS.Environment.DirectoryPictures);
            var cameraFile = new Java.IO.File(documentsDirectry, $"{DateTime.Now.ToString("yyMMddHHmmss")}.jpg");
            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                photoURI = FileProvider.GetUriForFile(this.mainActivity, this.mainActivity.PackageName + ".fileprovider", cameraFile);

            }
            else
            {
                photoURI = AndroidNet.Uri.FromFile(cameraFile);
            }

            intent.PutExtra(MediaStore.ExtraOutput, photoURI);
            mainActivity.StartActivityForResult(intent, 3); //拍照

        }

        /**
         * 录像
         */
        private void recordVideo()
        {
            Intent intent = new Intent(MediaStore.ActionVideoCapture);
            intent.PutExtra(MediaStore.ExtraVideoQuality, 1);
            //限制时长10秒
            intent.PutExtra(MediaStore.ExtraDurationLimit, 10);
            //最大10M
            intent.PutExtra(MediaStore.ExtraSizeLimit, 1024 * 1024 * 10);
            //开启摄像机,录制完成之后的操作，系统会默认把视频放到照片的文件夹中  
            mainActivity.StartActivityForResult(intent, 2);
        }
        AndroidNet.Uri photoUri;




        public override void OnPermissionRequest(PermissionRequest request)
        {
            string[] res = request.GetResources();
            foreach (string r in res)
            {

                if (mainActivity.CheckSelfPermission(r) != Permission.Granted)
                {
                    // Toast.MakeText(Application.Context, $"[{r}]未授权", ToastLength.Long).Show();
                    mainActivity.RequestPermissions(new String[] { r }, 1);
                }
            }

            request.Grant(res);
            //    base.OnPermissionRequest(request);
        }




        public override Boolean OnShowFileChooser(WebView webView, IValueCallback uploadMsg, WebChromeClient.FileChooserParams fileChooserParams)
        {
            mainActivity.mUploadMessage = uploadMsg;
            string[] acceptTypes = fileChooserParams.GetAcceptTypes();
            try
            {
                if (fileChooserParams.IsCaptureEnabled)
                {

                    if (acceptTypes[0].StartsWith("video"))
                    {
                        //openCamera();
                        recordVideo();
                    }
                    else
                    {
                        takePhoto();
                    }

                }
                else callback(uploadMsg, null, null);
            }
            catch (Exception e)
            {

            }
            return true;
        }
    }
    public class MauiBlazorWebViewHandler : BlazorWebViewHandler
    {
        protected override void ConnectHandler(Android.Webkit.WebView platformView)
        {
            base.ConnectHandler(platformView);
        }
    }
}
