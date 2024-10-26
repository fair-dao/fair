
using fairdao.extensions.shared.entity;
using fairdao.extensions.shared;
using fairdao.extensions.shared.entity;
using fairdao.extensions.shared.localization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace fairdao.extensions.shared
{

    public delegate void PageChangeed();
    public class SysHelper
    {

        /// <summary>
        /// 入口程序集
        /// </summary>
        public static Assembly EntryAssembly;

     
        /// <summary>
        /// 配置改变事件
        /// </summary>
        public delegate void ConfigChangedHandler();


        public event ConfigChangedHandler ConfigChanged;


        public static List<fairdao.extensions.shared.Extender> Extenders;

   


        public event PageChangeed OnPageChange;



        public static IServiceCollection Services { get; set; }
        /// <summary>
        /// App启动时间
        /// </summary>
        public static DateTime AppStartTime;

        /// <summary>
        /// Sqlite数据库文件
        /// </summary>
        public string SQLDbFile = "";


        /// <summary>
        /// 已运行的时间(毫秒数）
        /// </summary>
        public static long RunedSpanTime
        {
            get
            {
                return (long)(DateTime.Now - AppStartTime).TotalMilliseconds;
            }
        }

        /// <summary>
        /// 数据存储对象
        /// </summary>
        public IDataStore dataStore = null;

        /// <summary>
        /// 包装APIAddress
        /// </summary>
        /// <param name="action">API调用动作：如 user/delete</param>
        /// <returns></returns>
        public string WraperAPIActionUrl(string action)
        {
            return Path.Combine(ApiUrl, action);

        }

        public static SysHelper CurHelper;





        protected string appName;

        /// <summary>
        /// 程序名
        /// </summary>
        public virtual string AppName
        {
            get
            {
                if (string.IsNullOrEmpty(appName))
                {
                    appName = Assembly.GetEntryAssembly()?.GetName()?.Name;
                }
                return appName;
            }
        }


        public virtual string LangLibName
        {
            get
            {
                return "gensys";
            }
        }


        /// <summary>
        /// 机器时间与服务器时间相差的毫秒数
        /// </summary>

        public Int64 spanTime = Int64.MinValue;

        /// <summary>
        /// 服务器时间
        /// </summary>
        public DateTime? ServerTime
        {
            get
            {
                if (spanTime == Int64.MinValue)
                {
                    return null;
                }
                else
                {
                    return DateTime.Now.AddMilliseconds(spanTime);
                }

            }
        }



        /// <summary>
        /// 客户端配置 
        /// </summary>
        public fairdao.extensions.shared.entity.ClientConfig ClientConfig { get; set; }


        public SysHelper(IDataStore dataStore, fairdao.extensions.shared.entity.Env env)
        {
            this.dataStore = dataStore;
            this.ClientEnv = env;
        }




        public const string ConfigTableName = "Config";


        public Task SetCulture(string culture)
        {

            if (string.IsNullOrEmpty(culture))
            {
                culture = "zh";
            }
            else
            {


                if (culture.Length > 3)
                {
                    var c = culture.ToLower();

                    if (c.StartsWith("es-"))
                    {
                        culture = "es";
                    }
                    else if (c.StartsWith("en-"))
                    {
                        culture = "en";
                    }
                    else
                    {
                        culture = "zh";
                    }
                }

            }
            return dataStore.SetConfig("culture", new IndexedConfigData<string> { Id = "culture", Entity = culture });
        }

        string storeKey;


       
        public async Task<T> GetCache<T>(string cacheId)
        {
            var data = await dataStore.GetConfig<IndexedConfigData<T>>(cacheId);
            if (data == null) { return default(T); }
            return data.Entity ?? default(T);
        }


        public async Task SetCache(string cacheId, object data)
        {
            await dataStore.SetConfig(cacheId, new IndexedConfigData<object> { Id = cacheId, Entity = data });

        }

        /// <summary>
        /// 页面数据缓存
        /// </summary>
        public SortedDictionary<string, object> PageDataCaches = new();

        public async Task<string> GetCulture()
        {
            try
            {
                var data = await dataStore.GetConfig<IndexedConfigData<string>>("culture");
                return data?.Entity;
            }
            catch
            {
                return null;
            }
        }



        public virtual Task Init()
        {
            return Task.CompletedTask;


        }



        public string Concat(object s1, object s2)
        {
            return $"{s1}{s2}";
        }




        public Task<string> GetString(IAPIHttpClient httpClient, string url)
        {
            return httpClient.GetString(url);
        }


        public delegate void PostBack<T>(T obj);






        /// <summary>
        /// 将日期输出为UTC格式，方便API调用
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToUTCStr(DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            else
            {
                return dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
        }


        /// <summary>
        /// 将日期输出为UTC格式，方便API调用
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToUTCStr(DateTimeOffset dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        /// <summary>
        /// 显示本地时间
        /// </summary>
        /// <param name="time"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ShowLocalTime(DateTime time, string format = "datetime")
        {
            DateTime showTime = time.ToLocalTime();
            switch (format)
            {
                case "date":
                    return showTime.ToString("yy-MM-dd");
                case "datetme":
                    return showTime.ToString("yy-MM-dd HH:mm");
                    break;
                case "datetime_input":
                    return showTime.ToString("yy-MM-ddTHH:mm");
                    break;
            }
            return showTime.ToString();
        }






        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="audio"></param>
        /// <returns></returns>
        public virtual Task PlayAudio(string audio)
        {
            throw new Exception("未实现PlayAudio");
        }




        /// <summary>
        /// 是否在后台运行
        /// </summary>
        /// <returns></returns>
        public async virtual Task<bool> RunInBackground(IJSRuntime jSRuntime)
        {

            try
            {
                return await jSRuntime.InvokeAsync<bool>("gensys.env.RunInBackground");
            }
            catch (Exception ex)
            {
                return true;
            }
        }


        public async Task SetClientConfig(fairdao.extensions.shared.entity.ClientConfig config)
        {

            if (config != null)
            {

                try
                {
                    await dataStore.SetConfig(config.Id, config);
                    ClientConfig = config;

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
            else
            {
                await dataStore.RemoveConfig(ClientConfig.StoreId);
                ClientConfig = null;
            }

        }





        /// <summary>
        /// 客户端环境
        /// </summary>
        public entity.Env ClientEnv { get; set; }

        /// <summary>
        /// 本地化语言数据
        /// </summary>
        public LocalLang LocalData;

        private const string NAME_LOCALDATA_STOREID = "localdata";


        /// <summary>
        /// 设置多语言数据
        /// </summary>
        /// <returns></returns>
        public async Task SaveLocalData()
        {

            string lan = System.Text.Json.JsonSerializer.Serialize(LocalData.LangWords);
            await dataStore.SetConfig("cultures", new fairdao.extensions.shared.entity.IndexedConfigData<List<Culture>> { Id = "cultures", Entity = LocalData.Cultures });
            string langId = $"{NAME_LOCALDATA_STOREID}-{LocalData.LangWords.Code}";
            await dataStore.SetConfig(langId, new fairdao.extensions.shared.entity.IndexedConfigData<string> { Id = langId, Entity = lan });



        }


        /// <summary>
        /// 加载本地化数据
        /// </summary>
        /// <returns></returns>
        public async Task LoadLocalData()
        {
            string lang = await GetCulture();
            fairdao.extensions.shared.entity.IndexedConfigData<List<Culture>> cultures = await dataStore.GetConfig<fairdao.extensions.shared.entity.IndexedConfigData<List<Culture>>>($"cultures");
            fairdao.extensions.shared.entity.IndexedConfigData<string> langInfo = await dataStore.GetConfig<fairdao.extensions.shared.entity.IndexedConfigData<string>>($"{NAME_LOCALDATA_STOREID}-{lang}");
            LocalLang data = new LocalLang();
            //if (cultures?.Entity == null)
            //{
            //    cultures=new IndexedConfigData<List<Culture>>();
            //    cultures.Entity.Add(new Culture { DispName="中文", Name="zh" });
            //    cultures.Entity.Add(new Culture { DispName = "English", Name = "en" });
            //}
            data.Cultures = cultures?.Entity ?? new List<Culture>();
            
            LangInfo lan = null;
            if (!string.IsNullOrEmpty(langInfo?.Entity))
            {
                lan = System.Text.Json.JsonSerializer.Deserialize<LangInfo>(langInfo?.Entity);
            }

            data.LangWords = lan ?? new LangInfo();
            LocalData = data;
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} 本地数据:{LocalData}");
        }

        /// <summary>
        /// 获取当前语言
        /// </summary>
        /// <returns></returns>
        public async Task<Culture> GetCurLang()
        {
            string lang = await GetCulture();
            return LocalData.Cultures.FirstOrDefault(m => m.Name == lang);
        }




        public string ApiUrl { get; set; }


        /// <summary>
        /// 获取网址中指定Query字段值 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key">query字段</param>
        /// <remarks>
        /// 如：http://www.baidu.com/?q=aaa中取q字段的值aaa
        /// </remarks>
        /// <returns></returns>
        public string GetQuery(string url, string key)
        {

            if (url == null) return null;
            int qIndex = url.IndexOf('?');
            if (qIndex < 0) return null;
            string u2 = url.Substring(qIndex + 1);
            int index = 0;
            while (index >= 0)
            {
                string query = key + "=";  //如back=du
                index = u2.IndexOf(query, index);  //0
                if (index < 0) break;
                if (index == 0 || u2[index - 1] == '&') //找到
                {
                    int count = index + query.Length;//值起始位置,
                    int index2 = u2.IndexOf('&', index + query.Length);
                    if (index2 == 0)
                    {
                        return string.Empty;
                    }

                    if (index2 < 0) //后面没有其它query了
                    {
                        //找锚标记 #， 取#前面的数据
                        string data = u2.Substring(count);
                        index = data.IndexOf("#");
                        if (index >= 0)
                        {
                            return data.Substring(0, index);
                        }
                        else return data;
                    }
                    else
                    {
                        return u2.Substring(count, index2 - count);
                    }
                }

            }
            return null;



        }


        /// <summary>
        /// 获取Query值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetQuery(string key, NavigationManager nav = null)
        {
            nav = nav ?? Services.BuildServiceProvider().GetService<NavigationManager>();
            return GetQuery(nav.Uri, key);
        }

        /// <summary>
        /// 显示时间
        /// </summary>
        /// <param name="time"></param>
        /// <param name="type"></param>
        /// <returns></returns>

        public string ShowDateTime(DateTime time, DateTimeType type = DateTimeType.YMDHM)
        {
            if (time.Kind == DateTimeKind.Utc)
            {
                time = time.ToLocalTime();
            }

            switch (type)
            {
                case DateTimeType.YMD:
                    return time.ToString("yyyy-MM-dd");
                case DateTimeType.MDHM:
                    return time.ToString("MM-dd HH:mm");
                case DateTimeType.MDHMS:
                    return time.ToString("MM-dd HH:mm:ss");
                case DateTimeType.MDTHM: //带T的格式
                    return time.ToString("MM-ddTHH:mm");
                case DateTimeType.YMDTHM: //带T的格式
                    return time.ToString("yyyy-MM-ddTHH:mm");
                default: //年-月-日 时:分                    
                    return time.ToString("yyyy-MM-dd HH:mm");
            }

        }





        public string Post(string url, string contentType, string data)
        {
            string cType = "application/json";
            switch (contentType)
            {
                case "form":
                    cType = "application/x-www-form-urlencoded";
                    break;
            }
            var data1 = Encoding.UTF8.GetBytes(data);
            var content = new ByteArrayContent(data1);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse(cType);
            var response = new HttpClient().PostAsync(url, content).Result;
            return response.Content.ReadAsStringAsync().Result;

        }

        public async Task InitCulture()
        {
            #region 本地化
            var jsInterop = Services.BuildServiceProvider().GetRequiredService<IJSRuntime>();

            if (jsInterop == null)
            {
                Console.WriteLine("js运行时获取失败,本地化语言处理失败");
            }
            else
            {
                try
                {
                    string lang = await GetCulture();
                    if (string.IsNullOrEmpty(lang))
                    {
                        lang = "zh";
                        await SetCulture(lang);
                    }

                    Console.Write($"lang:{lang}");

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("读取blazorCulture失败");
                }
            }


            #endregion

        }

        public async Task ChangeLang(fairdao.extensions.shared.services.TransService trans, GensysLocaler localer, string lang, bool must = false)
        {
            string oldLang = await GetCulture();
            if (oldLang != lang || must)
            {
                LocalData = await trans.GetAllLocalWords(LangLibName, lang);
                await SaveLocalData();
                await SetCulture(lang);
                localer.CurLang = await GetCurLang();
                /*将翻译库加载到翻译器里*/
                localer.Langs = new SortedList<string, LangInfo>();
                if (!String.IsNullOrWhiteSpace(LocalData?.LangWords?.Code))
                {
                    localer.Langs.Add(LocalData.LangWords.Code, LocalData.LangWords);
                }
            }
        }

        public async Task ReloadConfig()
        {
            var config = new ClientConfig();
            //sysHelper.ClientConfig.IceServers = config.IceServers;

            try
            {

                List<VCommpent> tabs = new List<VCommpent>();
                List<VCommpent> sides = new List<VCommpent>();
                //应用插件配置
                Extenders?.ForEach(extender =>
                {
                    //处理可视组件
                    if (extender.VCommpents != null)
                    {
                        foreach (var com in extender.VCommpents)
                        {
                            if (com.Parent == VCommpent.Page)
                            {
                                var oldCom = tabs.FirstOrDefault(m => m.Id == com.Id);
                                if (oldCom == null)
                                {
                                    tabs.Add(com);
                                }
                                else
                                {
                                    if (com.SubMenus?.Count > 0)
                                    {
                                        oldCom.SubMenus.AddRange(com.SubMenus);
                                    }
                                }
                            }
                            else if (com.Parent == VCommpent.Sidebar)
                            {
                                var side = sides.FirstOrDefault(m => m.Id == com.Id);
                                if (side == null)
                                {
                                    sides.Add(com);
                                }
                                else
                                {
                                    if (com.SubMenus?.Count > 0)
                                    {
                                        side.SubMenus.AddRange(com.SubMenus);
                                    }
                                }
                            }
                            else
                            {
                                var comParent = tabs.FirstOrDefault(m => m.Id == com.Parent);
                                if (comParent == null) sides.FirstOrDefault(m => m.Id == com.Parent);
                                if (comParent == null)
                                {
                                    foreach (var tab in tabs)
                                    {
                                        comParent = tab.SubMenus.FirstOrDefault(sub => sub.Id == com.Parent);
                                        if (comParent != null) { break; }
                                    }
                                    if (comParent == null)
                                    {
                                        foreach (var side in sides)
                                        {
                                            comParent = side.SubMenus.FirstOrDefault(sub => sub.Id == com.Parent);
                                            if (comParent != null) { break; }
                                        }
                                    }
                                }
                                if (comParent != null)
                                {
                                    if (comParent.SubMenus == null)
                                    {
                                        comParent.SubMenus = new List<VCommpent>();
                                    }
                                    comParent.SubMenus.Add(com);
                                }

                            }
                        }
                    }

                });
                tabs.Sort((a, b) => (a.SortId ?? 0).CompareTo(b.SortId ?? 0));
                config.MTabs = tabs;
                config.Sides = sides;
                await SetClientConfig(config);
                ClientConfig = config;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }



    public static class SysHelperExt
    {
        public static async Task<IServiceProvider> UseHelper(this IServiceProvider provider)
        {
            var helper = provider.GetService<SysHelper>();
            var http = provider.GetService<IAPIHttpClient>();
            await helper.Init();
        

            await helper.InitCulture();
            var localer = provider.GetService<GensysLocaler>();
            localer.CurLang = await helper.GetCurLang();
            /*将翻译库加载到翻译器里*/
            localer.Langs = new SortedList<string, LangInfo>();
            if (!String.IsNullOrWhiteSpace(helper.LocalData?.LangWords?.Code))
            {
                localer.Langs.Add(helper.LocalData.LangWords.Code, helper.LocalData.LangWords);
            }
            return provider;

        }
    }
    public enum DateTimeType
    {
        /// <summary>
        /// 年-月-日 时:分:秒
        /// </summary>
        YMDHMS = 0,
        /// <summary>
        /// 年-月-日T时:分，本地时间格式
        /// </summary>
        YMDTHM = 5,
        /// <summary>
        /// 年-月-日T时:分，本地时间格式
        /// </summary>
        MDTHM = 6,


        /// <summary>
        /// 年-月-日 时:分
        YMDHM = 1,
        /// <summary>
        /// 年-月-日
        YMD = 2,
        /// <summary>
        /// 月-日 时:分
        /// </summary>
        MDHM = 3,
        /// <summary>
        /// 月-日 时:分:秒
        /// </summary>
        MDHMS = 4
    }



}