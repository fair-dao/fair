using fair.extensions.shared.localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fair.extensions.shared.services
{
    public class NewWordRecorder : INewWordRecorder
    {

        static SortedList<string, SortedList<string, string>> Recordeds = new SortedList<string, SortedList<string, string>>();

        private fair.extensions.shared.services.TransService trans;

        SysHelper helper;
        public NewWordRecorder(fair.extensions.shared.services.TransService transService,SysHelper helper)
        {

            this.trans = transService;
            this.helper = helper;
        }


        public void Record(string lang, string word, string plug, string page)
        {
            //站点管理员角色或测试用户才能提交到服务器自动翻译
            if ( page!="login") return;
            if (Recordeds.ContainsKey(lang) && Recordeds[lang].ContainsKey(word)) { return; }
            else
            {
                try
                {

                    if (!Recordeds.ContainsKey(lang))
                    {
                        var a = new SortedList<string, string>();
                        a.Add(word, null);
                        Recordeds.TryAdd(lang, a);
                    }
                    else Recordeds[lang].Add(word, null);
                }
                catch (Exception e)
                {

                }
                trans.AddTrans(lang, word, "",  helper.LangLibName);
            }
        }
    }

}
