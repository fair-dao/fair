using fair.extensions.shared.entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

namespace fair.extensions.shared.localization
{


    /// <summary>
    /// 本地化
    /// </summary>
    public class GensysLocaler
    {     
         
        private IStringLocalizer localizer;



        private string _AppName;
        private IServiceProvider provider;


        public SortedList<string,LangInfo> Langs { get; set; }

        /// <summary>
        /// 是否触发Get事件
        /// </summary>
        bool GetHander = false;

        /// <summary>
        /// name内容使用的语言
        /// </summary>
        private string DefaultLang;
        private string cn = "zh";

        public Culture CurLang { get; set; }

        private INewWordRecorder recorder;

        //private IStringLocalizer defaultLocaler;
        public GensysLocaler(IStringLocalizer<GensysLocaler> stringLocalizer,INewWordRecorder recorder)
        {
          
            localizer = stringLocalizer;
            this.recorder = recorder;
            if (string.IsNullOrEmpty(_AppName))
            {
                _AppName = Assembly.GetEntryAssembly()?.GetName()?.Name;
            }
      
            //var fact = p.GetRequiredService<IStringLocalizerFactory>();
            ////defaultLocaler = fact.Create("Share", Assembly.GetExecutingAssembly().FullName) ;
        }

        bool isInited = false;

        public string this[string name,string plug,string page=null]
        {
            get
            {
                try
                {
                    if (Langs == null) { return name; }
                    string lang = CurLang?.Name;
                    if (string.IsNullOrEmpty(lang)) lang = cn;
                    string v = null;
                    if (!Langs.ContainsKey(lang))
                    {
                        try
                        {
                            Langs.Add(lang, new LangInfo());
                        }
                        catch
                        {

                        }
                    }
                    var curLang = Langs[lang];

                    if (curLang.Dict != null && curLang.Dict.ContainsKey(name))
                    {
                        v = curLang.Dict[name];
                    }
                    //  else v = localizer[name];

                    if (string.IsNullOrEmpty(v))
                    { //记录下来                        
                        recorder.Record(lang, name,plug,page);
                        v = name;
                    }
                    //    Console.WriteLine($"翻译{lang}, {name},{v}");
                    return v;
                }
                catch
                {

                    return name;
                }
            }
        }


        public string this[string name]
        {
            get
            {
                return this[name, null, null];
            }
       
        }



        public string this[string name,object[] paras]
        {
            get
            {  
                string d = localizer[name,paras];
                return d;
            }
        }




    }
}
