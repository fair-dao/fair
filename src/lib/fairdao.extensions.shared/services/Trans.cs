using fairdao.extensions.shared;
using fairdao.extensions.shared.entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace fairdao.extensions.shared.services
{

    /// <summary>
    /// 本地化翻译服务
    /// </summary>
    public class TransService
    {
        

        private SysHelper sysHelper;

        private IAPIHttpClient httpClient;

        public TransService(SysHelper sysHelper,IAPIHttpClient httpClient)
        {
       
            this.sysHelper = sysHelper;
            this.httpClient = httpClient;
        }

        /// <summary>
        /// 写入翻译内容
        /// </summary>
        /// <param name="srcLang">原语言(如:zh-CN)</param>
        /// <param name="desLang">目标语言(如:en-US)</param>
        /// <param name="word">原短语(如:文件)</param>
        /// <param name="desWord">翻译结果(如:File)</param>
        /// <param name="lib">库(如:gensysclient)</param>
        /// <param name="rewrite">是否覆盖</param>
        /// <returns></returns>
        public async Task  AddTrans(string lang,string code,string word,string lib,bool rewrite=false)
        {
            //将翻译结果保存
            try
            {

                    Console.WriteLine($"翻译：{lang},{code},{word}");
                    await httpClient.SubmitResult<string>($"{sysHelper.ApiUrl}Tool/WriteTrans", $"lang={lang}&code={code}&word={word}&reWrite={rewrite}&lib={lib}");
               

            }
            catch (Exception e)
            {

            }

        }

        public async Task DelCode(string code, string lang ,string lib)
        {
            //将翻译结果保存
            try
            {
     
                    await httpClient.SubmitResult<string>($"{sysHelper.ApiUrl}Tool/DelCode", $"code={code}&lib={lib}&lang={lang}");
            }
            catch (Exception e)
            {

            }

        }




        public async Task<LocalLang> GetAllLocalWords(string lib,string lang)
        {
       
                var result = await httpClient.SubmitResult<LocalLang>($"{sysHelper.ApiUrl}Tool/GetAllLocalWords/{lib}/{lang}");
                return result;
           
        }
    }
}
